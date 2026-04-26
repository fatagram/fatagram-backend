using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.Domain.Models;
using Fatagram.Infrastructure.Cache;
using Fatagram.Infrastructure.Data;
using Fatagram.Infrastructure.Projections;
using Fatagram.Infrastructure.Repositories.BaseRepository;
using Fatagram.Infrastructure.Repositories.BaseRepository.Interfaces;
using Fatagram.Infrastructure.Repositories.ConversationRepository.Interfaces;
using Fatagram.Infrastructure.Repositories.MessageRepository.Interfaces;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace Fatagram.Infrastructure.Repositories.MessageRepository
{
    public class CachedMessageRepository : BaseRepositoryDecorator<Message>, IMessageRepository
    {
        private readonly ICacheService _cacheService;
        private readonly IConversationRepository _conversationRepository;

        public CachedMessageRepository(
            AppDbContext dbContext,
            IBaseRepository<Message> inner,
            ICacheService cacheService,
            IConversationRepository conversationRepository
        )
            : base(inner, dbContext)
        {
            _cacheService = cacheService;
            _conversationRepository = conversationRepository;
        }

        public override async Task<Message> AddAsync(Message entity)
        {
            entity.Id = Guid.NewGuid();
            entity.CreatedAt = DateTime.UtcNow;

            var lastMessageNumber = await _conversationRepository.IncreaseLastMessageNumberAsync(
                entity.ConversationId
            );
            entity.SequenceNumber = lastMessageNumber;

            var conversationId = entity.ConversationId;

            var messageZSetKey = $"conv:{conversationId}:messages";
            var mediaMapKey = $"conv:{conversationId}:media:map";
            var mediaTimelineKey = $"conv:{conversationId}:media:timeline";

            if (entity.Media != null && entity.Media.Count > 0)
            {
                int i = 0;
                var hashEntries = new List<HashEntry>();

                foreach (var media in entity.Media)
                {
                    media.Id = Guid.NewGuid();
                    media.CreatedAt = entity.CreatedAt;
                    media.IndexInMessage = i++;
                    media.MessageSequence = entity.SequenceNumber;

                    await _cacheService.SortedSetAddAsync(
                        mediaTimelineKey,
                        media,
                        entity.SequenceNumber
                    );

                    hashEntries.Add(
                        new HashEntry(
                            media.Id.ToString(),
                            $"{entity.SequenceNumber}:{media.IndexInMessage}"
                        )
                    );
                }

                await _cacheService.SortedSetAddAsync(
                    messageZSetKey,
                    entity,
                    entity.SequenceNumber
                );

                await _cacheService.HashSetAsync(mediaMapKey, [.. hashEntries]);
            }
            else
            {
                await _cacheService.SortedSetAddAsync(
                    messageZSetKey,
                    entity,
                    entity.SequenceNumber
                );
            }

            return entity;
        }

        public async Task<LastMessageProjection?> GetLastMessageOfConversationAsync(
            Guid conversationId
        )
        {
            string redisKey = $"conv:{conversationId}:messages";
            var cachedList = await _cacheService.SortedSetRangeByScoreAsync<Message>(
                redisKey,
                order: Order.Descending,
                take: 1
            );
            var lastMessage = cachedList?.FirstOrDefault();
            if (lastMessage != null)
                return new LastMessageProjection
                {
                    Id = lastMessage.Id,
                    ConversationId = lastMessage.ConversationId,
                    SenderId = lastMessage.SenderId,
                    SenderFullName = lastMessage.Sender?.FullName,
                    Content = lastMessage.Content,
                    SequenceNumber = lastMessage.SequenceNumber,
                    CreatedAt = lastMessage.CreatedAt,
                    Type = lastMessage.Type,
                    Media = lastMessage.Media,
                    Metadata = lastMessage.Metadata,
                };

            var inner = (IMessageRepository)_inner;
            return await inner.GetLastMessageOfConversationAsync(conversationId);
        }

        public async Task<List<Message>> GetMessages(
            Guid conversationId,
            int? cursor,
            bool desc,
            int limit
        )
        {
            Console.WriteLine(
                $"Getting messages for conversation {conversationId}, cursor: {cursor}, desc: {desc}, limit: {limit}"
            );
            var messagesKey = $"conv:{conversationId}:messages";
            Order redisOrder = desc ? Order.Descending : Order.Ascending;

            double start = double.NegativeInfinity;
            double stop = double.PositiveInfinity;
            Exclude exclude = Exclude.None;

            if (cursor.HasValue && cursor > 0)
            {
                if (desc)
                {
                    stop = cursor.Value;
                    exclude = Exclude.Stop;
                }
                else
                {
                    start = cursor.Value;
                    exclude = Exclude.Start;
                }
            }

            var pendingMessages = await _cacheService.SortedSetRangeByScoreAsync<Message>(
                messagesKey,
                start,
                stop,
                exclude,
                redisOrder,
                offset: 0,
                take: limit
            );

            Console.WriteLine(
                $"Cache returned {pendingMessages.Count} messages for conversation {conversationId}"
            );

            int remainingLimit = limit - pendingMessages.Count;

            if (remainingLimit <= 0)
            {
                return pendingMessages;
            }

            int? cursorForDb =
                pendingMessages.Count > 0 ? pendingMessages.Last().SequenceNumber : cursor;

            Console.WriteLine(
                $"Fetching from database with cursor {cursorForDb} for conversation {conversationId} remaining limit {remainingLimit}"
            );

            var messageRepo = (IMessageRepository)_inner;

            var messagesFromDb = await messageRepo.GetMessages(
                conversationId,
                cursorForDb,
                desc,
                remainingLimit
            );

            Console.WriteLine(
                $"Database returned {messagesFromDb.Count} messages for conversation {conversationId}"
            );

            return [.. pendingMessages, .. messagesFromDb];
        }

        public async Task<List<Message>> GetDeltaMessagesAsync(
            Guid conversationId,
            int sinceSequenceNumber
        )
        {
            var messagesKey = $"conv:{conversationId}:messages";

            var pendingMessages = await _cacheService.SortedSetRangeByScoreAsync<Message>(
                messagesKey,
                start: sinceSequenceNumber,
                stop: double.PositiveInfinity,
                exclude: Exclude.Start,
                order: Order.Ascending
            );

            var messageRepo = (IMessageRepository)_inner;
            var messagesFromDb = await messageRepo.GetDeltaMessagesAsync(
                conversationId,
                sinceSequenceNumber
            );

            var pendingSeqs = pendingMessages.Select(m => m.SequenceNumber).ToHashSet();

            return
            [
                .. pendingMessages,
                .. messagesFromDb.Where(m => !pendingSeqs.Contains(m.SequenceNumber)),
            ];
        }
    }
}

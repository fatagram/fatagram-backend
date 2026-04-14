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

            await _cacheService.ListRightPushAsync(
                $"conv:{entity.ConversationId}:messages",
                entity
            );

            return entity;
        }

        public async Task<LastMessageProjection?> GetLastMessageOfConversationAsync(
            Guid conversationId
        )
        {
            string redisKey = $"conv:{conversationId}:messages";
            var cachedList = await _cacheService.ListRangeAsync<Message>(redisKey, -1, -1);
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
            var messagesKey = $"conv:{conversationId}:messages";

            var cachedMessages = await _cacheService.ListRangeAsync<Message>(messagesKey, 0, -1);

            var pendingMessages = cachedMessages
                .Where(m =>
                    cursor == null
                    || cursor <= 0
                    || m.SequenceNumber < (cursor > 0 ? cursor : int.MaxValue)
                )
                .OrderByDescending(m => m.SequenceNumber)
                .Take(limit)
                .ToList();

            int remainingLimit = limit - pendingMessages.Count;

            if (remainingLimit <= 0)
            {
                return pendingMessages;
            }

            int? cursorForDb = pendingMessages.Any()
                ? pendingMessages.Min(m => m.SequenceNumber)
                : cursor;

            var messageRepo = (IMessageRepository)_inner;

            var messagesFromDb = await messageRepo.GetMessages(
                conversationId,
                cursorForDb,
                desc,
                remainingLimit
            );

            return [.. pendingMessages, .. messagesFromDb];
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.Domain.Models;
using Fatagram.Application.Abstractions.Cache;
using Fatagram.Infrastructure.Data;
using Fatagram.Application.Common.Projections;
using Fatagram.Infrastructure.Repositories.BaseRepository;
using Fatagram.Application.Abstractions.Repositories;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace Fatagram.Infrastructure.Repositories.MessageRepository
{
    public class CachedMessageRepository : BaseRepositoryDecorator<Message>, IMessageRepository
    {
        private readonly ICacheService _cacheService;
        private readonly IConversationRepository _conversationRepository;

        // Keep the 100 most recent messages per conversation in Redis.
        private const int HotWindowSize = 100;
        private static readonly TimeSpan HotWindowTtl = TimeSpan.FromHours(2);

        public CachedMessageRepository(
            AppDbContext dbContext,
            IMessageRepository inner,
            ICacheService cacheService,
            IConversationRepository conversationRepository
        )
            : base(inner, dbContext)
        {
            _cacheService = cacheService;
            _conversationRepository = conversationRepository;
        }

        private static string HotKey(Guid conversationId) => $"conv:{conversationId}:recent_msgs";

        public override async Task<Message> AddAsync(Message entity)
        {
            entity.Id = Guid.NewGuid();
            entity.CreatedAt = DateTime.UtcNow;
            entity.SequenceNumber = await _conversationRepository.IncreaseLastMessageNumberAsync(
                entity.ConversationId
            );

            var saved = await base.AddAsync(entity);

            // Invalidate the hot window so the next read repopulates from DB with fresh data.
            // Write path never touches Redis directly — cache is for reads only.
            await _cacheService.RemoveAsync(HotKey(entity.ConversationId));

            return saved;
        }

        public async Task<LastMessageProjection?> GetLastMessageOfConversationAsync(
            Guid conversationId
        )
        {
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
            // Only serve from hot window for the common case: newest-first, cursor within window.
            // ASC pagination (load from beginning) and deep-scroll always fall back to DB.
            if (desc)
            {
                var hotKey = HotKey(conversationId);
                var windowCount = await _cacheService.SortedSetLengthAsync(hotKey);

                if (windowCount > 0)
                {
                    // Cursor: exclusive upper bound on SequenceNumber (null = +∞ = latest).
                    double upperBound = cursor.HasValue
                        ? cursor.Value - 1
                        : double.PositiveInfinity;

                    // Determine the lowest seq in the hot window to know if we can serve from cache.
                    var oldest = await _cacheService.SortedSetRangeByScoreAsync<Message>(
                        hotKey,
                        order: Order.Ascending,
                        take: 1
                    );
                    int windowFloor = oldest.FirstOrDefault()?.SequenceNumber ?? int.MaxValue;

                    // Can serve from cache only if the requested range is fully inside the window.
                    bool cursorWithinWindow = !cursor.HasValue || cursor.Value > windowFloor;

                    if (cursorWithinWindow)
                    {
                        var cached = await _cacheService.SortedSetRangeByScoreAsync<Message>(
                            hotKey,
                            start: double.NegativeInfinity,
                            stop: upperBound,
                            order: Order.Descending,
                            take: limit
                        );

                        if (cached.Count == limit)
                            return cached;

                        // Partial hit: cached has fewer than `limit` items.
                        // If the window is exhausted (oldest item is the conversation's first message)
                        // return what we have; otherwise the cursor scrolled below the window floor
                        // and we fall through to DB.
                        if (cached.Count > 0 && windowFloor == 1)
                            return cached;
                    }
                }
            }

            // Fallback: read from DB.
            var innerRepo = (IMessageRepository)_inner;
            var dbMessages = await innerRepo.GetMessages(conversationId, cursor, desc, limit);

            // Lazy-populate the hot window when loading the latest page (desc, no meaningful cursor)
            // so subsequent reads for the same conversation can be served from cache.
            bool isLatestPage = desc && (!cursor.HasValue || cursor.Value >= int.MaxValue);
            if (isLatestPage && dbMessages.Count > 0)
            {
                var hotKey = HotKey(conversationId);
                foreach (var msg in dbMessages)
                    await _cacheService.SortedSetAddAsync(hotKey, msg, msg.SequenceNumber);
                await _cacheService.SortedSetRemoveRangeByRankAsync(
                    hotKey,
                    0,
                    -(HotWindowSize + 1)
                );
                await _cacheService.KeyExpireAsync(hotKey, HotWindowTtl);
            }

            return dbMessages;
        }

        public async Task<List<Message>> GetDeltaMessagesAsync(
            Guid conversationId,
            int sinceSequenceNumber
        )
        {
            var hotKey = HotKey(conversationId);
            var windowCount = await _cacheService.SortedSetLengthAsync(hotKey);

            if (windowCount > 0)
            {
                var oldest = await _cacheService.SortedSetRangeByScoreAsync<Message>(
                    hotKey,
                    order: Order.Ascending,
                    take: 1
                );
                int windowFloor = oldest.FirstOrDefault()?.SequenceNumber ?? int.MaxValue;

                if (sinceSequenceNumber >= windowFloor)
                {
                    return await _cacheService.SortedSetRangeByScoreAsync<Message>(
                        hotKey,
                        start: sinceSequenceNumber + 1,
                        stop: double.PositiveInfinity,
                        order: Order.Ascending
                    );
                }
            }

            var inner = (IMessageRepository)_inner;
            return await inner.GetDeltaMessagesAsync(conversationId, sinceSequenceNumber);
        }
    }
}

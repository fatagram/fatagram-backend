using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Fatagram.Domain.Models;
using Fatagram.Infrastructure.Cache;
using Fatagram.Infrastructure.Data;
using Fatagram.Infrastructure.Projections;
using Fatagram.Infrastructure.Repositories.BaseRepository;
using Fatagram.Infrastructure.Repositories.BaseRepository.Interfaces;
using Fatagram.Infrastructure.Repositories.ConversationRepository.Interfaces;
using Fatagram.Shared.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.Extensions.Logging;

namespace Fatagram.Infrastructure.Repositories.ConversationRepository
{
    public class CachedConversationRepository
        : BaseRepositoryDecorator<Conversation>,
            IConversationRepository
    {
        private readonly ICacheService _cacheService;

        public CachedConversationRepository(
            AppDbContext dbContext,
            IConversationRepository conversationRepository,
            ICacheService cacheService
        )
            : base(conversationRepository, dbContext)
        {
            _cacheService = cacheService;
        }

        public async Task<ConversationProjection?> GetConversationById(
            Guid userId,
            Guid conversationId,
            Expression<Func<Conversation, ConversationProjection>>? selector = null
        )
        {
            var inner = (IConversationRepository)_inner;
            return await inner.GetConversationById(userId, conversationId, selector);
        }

        public async Task<ConversationProjection?> GetConversationWith(
            Guid userId,
            Guid targetUserId
        )
        {
            var inner = (IConversationRepository)_inner;
            return await inner.GetConversationWith(userId, targetUserId);
        }

        public async Task<List<ConversationProjection>> GetMyConversationsAsync(
            string userId,
            DateTime? cursor,
            int limit
        )
        {
            var inner = (IConversationRepository)_inner;
            var convs = await inner.GetMyConversationsAsync(userId, cursor, limit);
            var unreadConvs = await GetUnreadConversationsAsync(userId.ToGuid());
            var unreadDict = unreadConvs.ToDictionary(u => u.ConversationId, u => u.UnreadCount);

            foreach (var conv in convs)
            {
                if (unreadDict.TryGetValue(conv.Id, out int count))
                {
                    conv.UnreadMessageCount = count;
                }
                else
                {
                    conv.UnreadMessageCount = 0;
                }
            }

            return convs;
        }

        public async Task<List<ConversationSeenInfoProjection>> GetUnreadConversationsAsync(
            Guid userId
        )
        {
            var dataKey = $"user:{userId}:unread_convs";
            var statusKey = $"user:{userId}:unread_convs_status";
            bool isSynced = await _cacheService.ExistsAsync(statusKey);
            if (!isSynced)
            {
                var inner = (IConversationRepository)_inner;
                var unreadConvsFromDb = await inner.GetUnreadConversationsAsync(userId);

                if (unreadConvsFromDb.Count > 0)
                {
                    foreach (var conv in unreadConvsFromDb)
                    {
                        await _cacheService.HashSetAsync(
                            dataKey,
                            conv.ConversationId.ToString(),
                            conv.UnreadCount.ToString()
                        );
                    }
                }

                await _cacheService.SetAsync(statusKey, "synced", TimeSpan.FromDays(1));
                await _cacheService.KeyExpireAsync(dataKey, TimeSpan.FromDays(1));
            }

            var allUnreadEntries = await _cacheService.HashGetAllAsync(dataKey);

            return allUnreadEntries
                .Where(entry => int.Parse(entry.Value) > 0)
                .Select(entry => new ConversationSeenInfoProjection
                {
                    ConversationId = Guid.Parse(entry.Key),
                    UnreadCount = int.Parse(entry.Value),
                })
                .ToList();
        }

        public async Task<int> GetUnreadCountAsync(Guid userId)
        {
            return GetUnreadConversationsAsync(userId).Result.Count;
        }

        public async Task NotifyNewMessage(
            Guid conversationId,
            Guid senderId,
            List<Guid> participantIds
        )
        {
            foreach (var m in participantIds)
            {
                var unreadConvKey = $"user:{m}:unread_convs";
                if (m != senderId)
                {
                    await _cacheService.HashIncrementAsync(
                        unreadConvKey,
                        conversationId.ToString(),
                        1
                    );
                }
            }
        }
    }
}

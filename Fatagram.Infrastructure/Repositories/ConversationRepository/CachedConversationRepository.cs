using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Extensions.Logging;

namespace Fatagram.Infrastructure.Repositories.ConversationRepository
{
    public class CachedConversationRepository(
        AppDbContext dbContext,
        IConversationRepository conversationRepository,
        ICacheService cacheService
    )
        : BaseRepositoryDecorator<Conversation>(conversationRepository, dbContext),
            IConversationRepository
    {
        private readonly ICacheService _cacheService = cacheService;

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

        public async Task<int> GetLastMessageNumberAsync(Guid conversationId)
        {
            var key = $"conv:{conversationId}:seq";
            if (await _cacheService.ExistsAsync(key))
            {
                var cachedValue = await _cacheService.GetAsync<string>(key);
                if (int.TryParse(cachedValue, out int lastSeq))
                {
                    return lastSeq;
                }
            }

            var inner = (IConversationRepository)_inner;
            int lastMessageNumber = await inner.GetLastMessageNumberAsync(conversationId);

            await _cacheService.SetAsync(key, lastMessageNumber, TimeSpan.FromHours(1));

            return lastMessageNumber;
        }

        private static DateTime ToUtcDateTime(DateTime value)
        {
            return value.Kind switch
            {
                DateTimeKind.Utc => value,
                DateTimeKind.Local => value.ToUniversalTime(),
                _ => DateTime.SpecifyKind(value, DateTimeKind.Utc),
            };
        }

        private async Task<List<ConversationProjection>> GetMyTopConversationsFromCacheAsync(
            Guid userId,
            int? limit,
            DateTime cursor,
            bool desc = true
        )
        {
            var key = $"user:{userId}:conversations_rank";
            double? cursorTimestamp = null;
            if (cursor != DateTime.MinValue)
            {
                cursorTimestamp = new DateTimeOffset(
                    ToUtcDateTime(cursor)
                ).ToUnixTimeMilliseconds();
            }
            var redisResults = await _cacheService.SortedSetRangeByScoreWithCursorAsync<string>(
                key,
                cursorTimestamp,
                limit,
                desc
            );

            Console.WriteLine(
                $"Cache returned {redisResults?.Count ?? 0} conversations for user {userId} with cursor {cursor}"
            );

            if (redisResults == null || redisResults.Count == 0)
                return [];

            var convIds = redisResults.Select(r => r.Value).ToList();

            var inner = (IConversationRepository)_inner;
            var query = _dbContext
                .Conversations.Where(c => convIds.Contains(c.Id.ToString()))
                .Select(c => new ConversationProjection
                {
                    Id = c.Id,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt,
                    LastMessage = c
                        .Messages.OrderByDescending(m => m.CreatedAt)
                        .Select(m => new LastMessageProjection
                        {
                            Id = m.Id,
                            ConversationId = m.ConversationId,
                            SenderId = m.SenderId,
                            Type = m.Type,
                            Metadata = m.Metadata,
                            Content = m.Content,
                            CreatedAt = m.CreatedAt,
                            SenderFullName = m.Sender.FullName,
                            SenderNickname = m
                                .Sender.ConversationParticipants.Where(cp =>
                                    cp.ConversationId == c.Id && cp.UserId == m.SenderId
                                )
                                .Select(cp => cp.Nickname)
                                .FirstOrDefault(),
                        })
                        .FirstOrDefault(),
                    LastMessageNumber = c.LastMessageNumber,
                    IsGroup = c.IsGroup,
                    TopParticipantNames = c.IsGroup
                        ? c
                            .Participants.OrderBy(p => p.CreatedAt)
                            .Select(p => p.User!.FullName!)
                            .Take(2)
                            .ToList()
                        : null!,
                    OtherUserId = c.IsGroup
                        ? null
                        : c.Participants.OrderByDescending(p => p.UserId != userId)
                            .Select(p => (Guid?)p.UserId)
                            .FirstOrDefault()
                        ?? userId,
                    ParticipantCount = c.IsGroup ? c.Participants.Count() : null,
                    Name = c.IsGroup
                        ? c.Name
                        : c
                            .Participants.OrderByDescending(p => p.UserId != userId)
                            .Select(p => p.User!.FullName)
                            .FirstOrDefault(),
                    AvatarUrl = c.IsGroup
                        ? c.AvatarUrl
                        : c
                            .Participants.OrderByDescending(p => p.UserId != userId)
                            .Select(p => p.User!.Avatar)
                            .FirstOrDefault(),
                });

            var scoreMap = redisResults.ToDictionary(r => r.Value, r => r.Score);

            var conversations = await query.ToListAsync();

            var result = conversations
                .Select(c =>
                {
                    if (scoreMap.TryGetValue(c.Id.ToString(), out double score))
                    {
                        c.LastActiveAt = DateTimeOffset
                            .FromUnixTimeMilliseconds((long)score)
                            .UtcDateTime;
                    }
                    return c;
                })
                .OrderBy(c => c.LastActiveAt)
                .ToList();

            if (desc)
            {
                result.Reverse();
            }

            return result;
        }

        private async Task<List<Guid>> GetAllConversationIdsFromCacheAsync(Guid userId)
        {
            var key = $"user:{userId}:conversations_rank";
            var redisResults = await _cacheService.SortedSetRangeByScoreWithCursorAsync<string>(
                key,
                double.NegativeInfinity,
                desc: false
            );

            return redisResults?.Select(r => r.Value.ToGuid()).ToList() ?? [];
        }

        public async Task<List<ConversationProjection>> GetMyConversationsAsync(
            string userId,
            DateTime? cursor,
            int limit = 20,
            List<Guid>? notInConvIds = null
        )
        {
            var topConvsFromCache = await GetMyTopConversationsFromCacheAsync(
                userId.ToGuid(),
                limit,
                cursor ?? DateTime.UtcNow
            );
            if (topConvsFromCache.Count >= limit)
            {
                return topConvsFromCache;
            }

            var allIdsFromCache = await GetAllConversationIdsFromCacheAsync(userId.ToGuid());
            var remainingLimit = limit - topConvsFromCache.Count;
            DateTime? cursorForDb = null;
            if (topConvsFromCache.Count > 0)
            {
                cursorForDb = ToUtcDateTime(topConvsFromCache.Min(c => c.LastActiveAt));
            }
            else if (cursor.HasValue)
            {
                cursorForDb = ToUtcDateTime(cursor.Value);
            }

            var inner = (IConversationRepository)_inner;

            var dbConvs = await inner.GetMyConversationsAsync(
                userId,
                cursorForDb,
                remainingLimit,
                allIdsFromCache
            );

            var unreadConvs = await GetUnreadConversationsAsync(userId.ToGuid());
            var unreadDict = unreadConvs.ToDictionary(u => u.ConversationId, u => u.UnreadCount);

            List<ConversationProjection> result = [.. topConvsFromCache, .. dbConvs];

            foreach (var conv in result)
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

            return result;
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

            return
            [
                .. allUnreadEntries
                    .Where(entry => int.Parse(entry.Value) > 0)
                    .Select(entry => new ConversationSeenInfoProjection
                    {
                        ConversationId = Guid.Parse(entry.Key),
                        UnreadCount = int.Parse(entry.Value),
                    }),
            ];
        }

        public async Task<int> GetUnreadCountAsync(Guid userId)
        {
            return (await GetUnreadConversationsAsync(userId)).Count;
        }

        public async Task<int> IncreaseLastMessageNumberAsync(Guid conversationId)
        {
            var key = $"conv:{conversationId}:seq";

            var newValue = await _cacheService.IncrementAsync(key);
            if (newValue <= 1)
            {
                var inner = (IConversationRepository)_inner;
                var dbLastSeq = await inner.GetLastMessageNumberAsync(conversationId);
                newValue = await _cacheService.IncrementAsync(key, dbLastSeq);
                await _cacheService.KeyExpireAsync(key, TimeSpan.FromHours(1));
            }

            return (int)newValue;
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

        public async Task<List<ConversationProjection>> GetDeltaAsync(Guid userId, DateTime since)
        {
            var sinceUtc = ToUtcDateTime(since);

            var topConversationsFromCache = await GetMyTopConversationsFromCacheAsync(
                userId,
                null,
                sinceUtc,
                desc: false
            );

            var inner = (IConversationRepository)_inner;
            var dbConversations = await inner.GetDeltaAsync(userId, sinceUtc);

            HashSet<Guid> cacheConversationIds = [.. topConversationsFromCache.Select(c => c.Id)];

            List<ConversationProjection> mergedConversations =
            [
                .. topConversationsFromCache,
                .. dbConversations.Where(c => !cacheConversationIds.Contains(c.Id)),
            ];

            var unreadConvs = await GetUnreadConversationsAsync(userId);
            var unreadDict = unreadConvs.ToDictionary(u => u.ConversationId, u => u.UnreadCount);

            foreach (var conv in mergedConversations)
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

            return mergedConversations.OrderBy(c => c.LastActiveAt).ToList();
        }

        public async Task<List<ConversationProjection>> SearchConversations(
            Guid userId,
            string query,
            int limit,
            Guid? cursor = null
        )
        {
            var inner = (IConversationRepository)_inner;
            return await inner.SearchConversations(userId, query, limit, cursor);
        }
    }
}

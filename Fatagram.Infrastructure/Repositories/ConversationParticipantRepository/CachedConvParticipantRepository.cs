using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Fatagram.Domain.Models;
using Fatagram.Infrastructure.Cache;
using Fatagram.Infrastructure.Data;
using Fatagram.Infrastructure.Projections;
using Fatagram.Infrastructure.Repositories.BaseRepository;
using Fatagram.Infrastructure.Repositories.BaseRepository.Interfaces;
using Fatagram.Infrastructure.Repositories.ConversationParticipantRepository.Interfaces;
using Microsoft.EntityFrameworkCore;
using NpgsqlTypes;

namespace Fatagram.Infrastructure.Repositories.ConversationParticipantRepository
{
    public class CachedConvParticipantRepository
        : BaseRepositoryDecorator<ConversationParticipant>,
            IConversationParticipantRepository
    {
        private readonly ICacheService _cacheService;

        public CachedConvParticipantRepository(
            ICacheService cacheService,
            IBaseRepository<ConversationParticipant> inner,
            AppDbContext dbContext
        )
            : base(inner, dbContext)
        {
            _cacheService = cacheService;
        }

        private class SeenCache
        {
            public int Seq { get; set; }
            public DateTime At { get; set; }
        }

        public async Task<ParticipantsSeenProjection> GetConversationParticipantsSeenInfoAsync(
            Guid conversationId,
            List<Guid>? userIds = null
        )
        {
            var participants = await GetParticipantsAsync(conversationId);
            if (participants.Count == 0)
                return new() { ConversationId = conversationId, ParticipantsSeenInfo = new() };

            var tasks = participants.Select(async p =>
            {
                var lastReadKey = $"user:{p.UserId}:last_read";
                var seenData = await _cacheService.HashGetAsync(
                    lastReadKey,
                    conversationId.ToString()
                );

                if (!string.IsNullOrEmpty(seenData))
                {
                    var seenInfo = JsonSerializer.Deserialize<SeenCache>(seenData);
                    if (seenInfo != null)
                        return new
                        {
                            p.UserId,
                            SeenInfo = new ParticipantSeenInfoProjection
                            {
                                SequenceNumber = seenInfo?.Seq ?? 0,
                                SeenAt = seenInfo?.At ?? DateTime.UnixEpoch,
                            },
                            IsHit = true,
                        };
                }
                return new
                {
                    p.UserId,
                    SeenInfo = (ParticipantSeenInfoProjection)null!,
                    IsHit = false,
                };
            });

            var results = await Task.WhenAll(tasks);

            var dict = results.Where(r => r.IsHit).ToDictionary(r => r.UserId, r => r.SeenInfo);
            var cacheMissIds = results.Where(r => !r.IsHit).Select(r => r.UserId).ToList();

            if (cacheMissIds.Any())
            {
                var inner = (IConversationParticipantRepository)_inner;
                var getCacheMiss = await inner.GetConversationParticipantsSeenInfoAsync(
                    conversationId,
                    cacheMissIds
                );

                await Task.WhenAll(
                    getCacheMiss.ParticipantsSeenInfo.Select(kvp =>
                        _cacheService.HashSetAsync(
                            $"user:{kvp.Key}:last_read",
                            conversationId.ToString(),
                            JsonSerializer.Serialize(kvp.Value)
                        )
                    )
                );

                foreach (var kvp in getCacheMiss.ParticipantsSeenInfo)
                {
                    dict[kvp.Key] = kvp.Value;
                }
            }

            return new() { ConversationId = conversationId, ParticipantsSeenInfo = dict };
        }

        public async Task<List<ConversationParticipant>> GetParticipantsAsync(Guid conversationId)
        {
            var cacheKey = $"conversation:{conversationId}:participants";
            var cachedData = await _cacheService.GetAsync<List<ConversationParticipant>>(cacheKey);
            if (cachedData != null)
            {
                return cachedData;
            }
            var inner = (IConversationParticipantRepository)_inner;
            return await inner.GetParticipantsAsync(conversationId);
        }

        public async Task MarkAsSeenAsync(
            Guid conversationId,
            Guid userId,
            int messageSeq,
            DateTime seenAt
        )
        {
            var lastSeqKey = $"conv:{conversationId}:seq";
            var lastSeqRaw = await _cacheService.GetAsync<string>(lastSeqKey);
            int currentLastSeq;

            if (string.IsNullOrEmpty(lastSeqRaw))
            {
                currentLastSeq = await _dbContext
                    .Conversations.Where(c => c.Id == conversationId)
                    .Select(c => c.LastMessageNumber)
                    .FirstOrDefaultAsync();

                await _cacheService.SetAsync(lastSeqKey, currentLastSeq, TimeSpan.FromHours(24));
            }
            else
            {
                int.TryParse(lastSeqRaw, out currentLastSeq);
            }

            if (messageSeq > currentLastSeq)
            {
                messageSeq = currentLastSeq;
            }

            var lastReadKey = $"user:{userId}:last_read";
            var oldDataRaw = await _cacheService.HashGetAsync(
                lastReadKey,
                conversationId.ToString()
            );
            int oldSeenSeq = 0;

            if (string.IsNullOrEmpty(oldDataRaw))
            {
                oldSeenSeq = await _dbContext
                    .ConversationParticipants.Where(cp =>
                        cp.ConversationId == conversationId && cp.UserId == userId
                    )
                    .Select(cp => cp.LastSeenNumber)
                    .FirstOrDefaultAsync();
            }
            else
            {
                var oldInfo = JsonSerializer.Deserialize<SeenCache>(oldDataRaw);
                oldSeenSeq = oldInfo?.Seq ?? 0;
            }

            if (messageSeq <= oldSeenSeq)
            {
                return;
            }

            var unreadConvKey = $"user:{userId}:unread_convs";
            await _cacheService.HashSetAsync(unreadConvKey, conversationId.ToString(), "0");

            var seenData = new SeenCache { Seq = messageSeq, At = seenAt };
            await _cacheService.HashSetAsync(
                lastReadKey,
                conversationId.ToString(),
                JsonSerializer.Serialize(seenData)
            );
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Fatagram.Domain.Models;
using Fatagram.Application.Abstractions.Cache;
using Fatagram.Infrastructure.Data;
using Fatagram.Application.Common.Projections;
using Fatagram.Infrastructure.Repositories.BaseRepository;
using Fatagram.Application.Abstractions.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Fatagram.Infrastructure.Repositories.ConversationParticipantRepository
{
    public class CachedConvParticipantRepository
        : BaseRepositoryDecorator<ConversationParticipant>,
            IConversationParticipantRepository
    {
        private readonly ICacheService _cacheService;
        private readonly IConversationRepository _conversationRepository;

        public CachedConvParticipantRepository(
            ICacheService cacheService,
            IBaseRepository<ConversationParticipant> inner,
            IConversationRepository conversationRepository,
            AppDbContext dbContext
        )
            : base(inner, dbContext)
        {
            _cacheService = cacheService;
            _conversationRepository = conversationRepository;
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
                    var seenInfo = JsonSerializer.Deserialize<ParticipantSeenInfoProjection>(
                        seenData
                    );
                    if (seenInfo != null)
                        return new
                        {
                            p.UserId,
                            SeenInfo = new ParticipantSeenInfoProjection
                            {
                                SequenceNumber = seenInfo?.SequenceNumber ?? 0,
                                SeenAt = seenInfo?.SeenAt ?? DateTime.UnixEpoch,
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

        private static readonly TimeSpan ParticipantsCacheTtl = TimeSpan.FromMinutes(30);

        public async Task<List<ConversationParticipant>> GetParticipantsAsync(Guid conversationId)
        {
            var cacheKey = $"conversation:{conversationId}:participants";
            var cachedData = await _cacheService.GetAsync<List<ConversationParticipant>>(cacheKey);
            if (cachedData != null)
                return cachedData;

            var inner = (IConversationParticipantRepository)_inner;
            var participants = await inner.GetParticipantsAsync(conversationId);

            if (participants.Count > 0)
                await _cacheService.SetAsync(cacheKey, participants, ParticipantsCacheTtl);

            return participants;
        }

        public async Task<List<ConversationParticipant>> GetParticipantsAsync(
            Guid conversationId,
            DateTime? cursor,
            int limit
        )
        {
            var inner = (IConversationParticipantRepository)_inner;
            return await inner.GetParticipantsAsync(conversationId, cursor, limit);
        }

        public async Task MarkAsSeenAsync(
            Guid conversationId,
            Guid userId,
            int messageSeq,
            DateTime seenAt
        )
        {
            var currentLastSeq = await _conversationRepository.GetLastMessageNumberAsync(
                conversationId
            );

            if (messageSeq > currentLastSeq)
                messageSeq = currentLastSeq;

            var lastReadKey = $"user:{userId}:last_read";
            var oldDataRaw = await _cacheService.HashGetAsync(lastReadKey, conversationId.ToString());
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
                var oldInfo = JsonSerializer.Deserialize<ParticipantSeenInfoProjection>(oldDataRaw);
                oldSeenSeq = oldInfo?.SequenceNumber ?? 0;
            }

            if (messageSeq <= oldSeenSeq)
                return;

            // Write directly to DB
            await _dbContext
                .ConversationParticipants.Where(cp =>
                    cp.ConversationId == conversationId
                    && cp.UserId == userId
                    && cp.LastSeenNumber < messageSeq
                )
                .ExecuteUpdateAsync(s =>
                    s.SetProperty(cp => cp.LastSeenNumber, messageSeq)
                     .SetProperty(cp => cp.SeenAt, seenAt)
                );

            // Update read caches
            var unreadConvKey = $"user:{userId}:unread_convs";
            await _cacheService.HashSetAsync(
                unreadConvKey,
                conversationId.ToString(),
                (currentLastSeq - messageSeq).ToString()
            );

            await _cacheService.HashSetAsync(
                lastReadKey,
                conversationId.ToString(),
                JsonSerializer.Serialize(new ParticipantSeenInfoProjection
                {
                    SequenceNumber = messageSeq,
                    SeenAt = seenAt,
                })
            );
        }

        public async Task<bool> TogglePinAsync(Guid conversationId, Guid userId)
        {
            var inner = (IConversationParticipantRepository)_inner;
            return await inner.TogglePinAsync(conversationId, userId);
        }
    }
}

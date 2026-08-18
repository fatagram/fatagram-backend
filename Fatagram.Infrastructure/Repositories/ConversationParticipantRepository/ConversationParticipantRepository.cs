using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Fatagram.Domain.Models;
using Fatagram.Infrastructure.Data;
using Fatagram.Application.Common.Projections;
using Fatagram.Infrastructure.Repositories.BaseRepository;
using Fatagram.Application.Abstractions.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Fatagram.Infrastructure.Repositories.ConversationParticipantRepository
{
    public class ConversationParticipantRepository
        : BaseRepository<ConversationParticipant>,
            IConversationParticipantRepository
    {
        public ConversationParticipantRepository(
            AppDbContext dbContext,
            ILogger<BaseRepository<ConversationParticipant>>? logger = null
        )
            : base(dbContext, logger) { }

        public async Task<ParticipantsSeenProjection> GetConversationParticipantsSeenInfoAsync(
            Guid conversationId,
            List<Guid>? userIds = null
        )
        {
            var participantsSeenInfo = _dbContext
                .ConversationParticipants.Where(cp => cp.ConversationId == conversationId)
                .Select(cp => new
                {
                    cp.UserId,
                    seenInfo = new ParticipantSeenInfoProjection
                    {
                        SequenceNumber = cp.LastSeenNumber,
                        SeenAt = cp.SeenAt ?? DateTime.UnixEpoch,
                    },
                })
                .AsQueryable();

            if (userIds != null && userIds.Count > 0)
            {
                participantsSeenInfo = participantsSeenInfo.Where(p => userIds.Contains(p.UserId));
            }

            var result = await participantsSeenInfo.ToListAsync();

            return new()
            {
                ConversationId = conversationId,
                ParticipantsSeenInfo = result.ToDictionary(p => p.UserId, p => p.seenInfo),
            };
        }

        public async Task<List<ConversationParticipant>> GetParticipantsAsync(Guid conversationId)
        {
            return await _dbContext
                .ConversationParticipants.Where(cp => cp.ConversationId == conversationId)
                .Include(cp => cp.User)
                .ToListAsync();
        }

        public async Task<List<ConversationParticipant>> GetParticipantsAsync(
            Guid conversationId,
            DateTime? cursor,
            int limit
        )
        {
            var query = _dbContext
                .ConversationParticipants.Where(cp => cp.ConversationId == conversationId)
                .Include(cp => cp.User)
                .AsQueryable();

            if (cursor.HasValue && cursor.Value != DateTime.MinValue)
            {
                query = query.Where(cp => cp.CreatedAt > cursor.Value);
            }

            return await query.OrderBy(cp => cp.CreatedAt).Take(limit).ToListAsync();
        }

        public async Task MarkAsSeenAsync(
            Guid conversationId,
            Guid userId,
            int messageSeq,
            DateTime seenAt
        )
        {
            var conversationParticipant = await _dbContext
                .ConversationParticipants.Where(cp =>
                    cp.ConversationId == conversationId && cp.UserId == userId
                )
                .ExecuteUpdateAsync(s =>
                    s.SetProperty(cp => cp.LastSeenNumber, messageSeq)
                        .SetProperty(cp => cp.SeenAt, seenAt)
                );
        }

        public async Task<bool> TogglePinAsync(Guid conversationId, Guid userId)
        {
            var participant = await _dbContext.ConversationParticipants
                .FirstOrDefaultAsync(cp => cp.ConversationId == conversationId && cp.UserId == userId);

            if (participant == null)
                return false;

            if (participant.PinnedAt == null)
            {
                participant.PinnedAt = DateTime.UtcNow;
            }
            else
            {
                participant.PinnedAt = null;
            }

            await _dbContext.SaveChangesAsync();
            return participant.PinnedAt != null;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fatagram.Domain.Models;
using Fatagram.Application.Common.Projections;

namespace Fatagram.Application.Abstractions.Repositories
{
    public interface IConversationParticipantRepository : IBaseRepository<ConversationParticipant>
    {
        Task<List<ConversationParticipant>> GetParticipantsAsync(Guid conversationId);
        Task<List<ConversationParticipant>> GetParticipantsAsync(
            Guid conversationId,
            DateTime? cursor,
            int limit
        );

        Task MarkAsSeenAsync(Guid conversationId, Guid userId, int messageSeq, DateTime seenAt);

        Task<ParticipantsSeenProjection> GetConversationParticipantsSeenInfoAsync(
            Guid conversationId,
            List<Guid>? userIds = null
        );
    }
}

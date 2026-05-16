using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.Domain.Models;
using Fatagram.Infrastructure.Projections;
using Fatagram.Infrastructure.Repositories.BaseRepository.Interfaces;

namespace Fatagram.Infrastructure.Repositories.ConversationParticipantRepository.Interfaces
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

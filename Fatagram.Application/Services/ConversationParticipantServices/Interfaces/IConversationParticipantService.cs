using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.Application.Dtos.Conversation;
using Fatagram.Application.Utils;
using Fatagram.Infrastructure.Projections;

namespace Fatagram.Application.Services.ConversationServices.Interfaces
{
    public interface IConversationParticipantService
    {
        Task<Result> MarkAsSeenAsync(Guid conversationId, Guid userId, int messageSeq);
        Task<Result<ParticipantsSeenProjection>> GetParticipantSeenAsync(Guid conversationId);
    }
}

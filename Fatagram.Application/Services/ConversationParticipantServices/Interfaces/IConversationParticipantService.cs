using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.Application.Utils;

namespace Fatagram.Application.Services.ConversationServices.Interfaces
{
    public interface IConversationParticipantService
    {
        Task<Result> MarkAsReadAsync(Guid conversationId, Guid userId, Guid messageId);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.Application.Dtos.Conversation;
using Fatagram.Application.Dtos.Filter;
using Fatagram.Application.Utils;
using Fatagram.Domain.Models;

namespace Fatagram.Application.Services.ConversationServices.Interfaces
{
    public interface IConversationService
    {
        Task<Result<CursorResult<ConversationDto, DateTime>>> GetAllAsync(
            Guid userId,
            CursorFilter<DateTime>? cursor = null
        );
        Task<Result<ConversationDto>> GetByIdAsync(Guid userId, Guid conversationId);
        Task<Result<ConversationDto>> GetWithAsync(Guid userId, Guid targetUserId);
        Task<Result<ConversationDto>> CreateAsync(Guid creatorId, Guid otherUserId);
        Task<Result<Guid>> CreateGroupAsync(
            Guid creatorId,
            IEnumerable<Guid> participantIds,
            string? name
        );
    }
}

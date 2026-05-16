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
        Task<Result<int>> GetUnreadCountAsync(Guid userId);
        Task<Result<List<ConversationDto>>> GetDeltaAsync(Guid userId, DateTime since);

        Task<Result<Guid>> CreateGroupAsync(
            Guid creatorId,
            IEnumerable<Guid> participantIds,
            string? name
        );

        Task<Result> UpdateAsync(Guid conversationId, UpdateConversationDto conversationDto);
        Task<Result> UpdateAvatarAsync(Guid conversationId, string avatarUrl, Guid userId);
        Task<Result> UpdateNameAsync(Guid conversationId, string name, Guid userId);

        Task<Result<CursorResult<ConversationDto, Guid>>> SearchAsync(
            Guid userId,
            CursorFilter<Guid> filter
        );
    }
}

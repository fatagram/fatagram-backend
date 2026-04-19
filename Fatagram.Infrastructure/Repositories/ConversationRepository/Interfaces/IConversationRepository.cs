using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Fatagram.Domain.Models;
using Fatagram.Infrastructure.Projections;
using Fatagram.Infrastructure.Repositories.BaseRepository.Interfaces;
using Microsoft.EntityFrameworkCore.Update.Internal;

namespace Fatagram.Infrastructure.Repositories.ConversationRepository.Interfaces
{
    public interface IConversationRepository : IBaseRepository<Conversation>
    {
        Task<List<ConversationProjection>> GetMyConversationsAsync(
            string userId,
            DateTime? cursor,
            int limit,
            List<Guid>? notInConvIds = null
        );

        Task<ConversationProjection?> GetConversationWith(Guid userId, Guid targetUserId);
        Task<ConversationProjection?> GetConversationById(
            Guid userId,
            Guid conversationId,
            Expression<Func<Conversation, ConversationProjection>>? selector = null
        );

        Task<List<ConversationSeenInfoProjection>> GetUnreadConversationsAsync(Guid userId);
        Task<int> GetUnreadCountAsync(Guid userId);

        // Task<List<ConversationSeenInfoProjection>> GetConversationsSeenInfoAsync(Guid userId);

        Task NotifyNewMessage(Guid conversationId, Guid senderId, List<Guid> participantIds);
        Task<int> IncreaseLastMessageNumberAsync(Guid conversationId);
        Task<int> GetLastMessageNumberAsync(Guid conversationId);
        Task<List<ConversationProjection>> GetDeltaAsync(Guid userId, DateTime since);
    }
}

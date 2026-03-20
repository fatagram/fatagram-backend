using System;
using System.Collections.Generic;
using System.Linq;
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
            int limit
        );

        Task<ConversationProjection?> GetConversationWith(Guid userId, Guid targetUserId);
        Task<ConversationProjection?> GetConversationById(Guid userId, Guid conversationId);
    }
}

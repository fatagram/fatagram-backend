using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.Domain.Models;
using Fatagram.Infrastructure.Projections;
using Fatagram.Infrastructure.Repositories.BaseRepository.Interfaces;

namespace Fatagram.Infrastructure.Repositories.MessageRepository.Interfaces
{
    public interface IMessageRepository : IBaseRepository<Message>
    {
        Task<List<Message>> GetMessages(Guid conversationId, int? cursor, bool desc, int limit);

        Task<LastMessageProjection?> GetLastMessageOfConversationAsync(Guid conversationId);
    }
}

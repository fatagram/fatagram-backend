using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fatagram.Domain.Models;
using Fatagram.Application.Common.Projections;

namespace Fatagram.Application.Abstractions.Repositories
{
    public interface IMessageRepository : IBaseRepository<Message>
    {
        Task<List<Message>> GetMessages(Guid conversationId, int? cursor, bool desc, int limit);
        Task<List<Message>> GetDeltaMessagesAsync(Guid conversationId, int sinceSequenceNumber);
        Task<LastMessageProjection?> GetLastMessageOfConversationAsync(Guid conversationId);
    }
}

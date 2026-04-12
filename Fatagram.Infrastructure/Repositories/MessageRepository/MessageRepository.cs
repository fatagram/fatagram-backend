using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Fatagram.Domain.Models;
using Fatagram.Infrastructure.Data;
using Fatagram.Infrastructure.Projections;
using Fatagram.Infrastructure.Repositories.BaseRepository;
using Fatagram.Infrastructure.Repositories.MessageRepository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Fatagram.Infrastructure.Repositories.MessageRepository
{
    public class MessageRepository : BaseRepository<Message>, IMessageRepository
    {
        public MessageRepository(
            AppDbContext dbContext,
            ILogger<BaseRepository<Message>>? logger = null
        )
            : base(dbContext, logger) { }

        public async Task<LastMessageProjection?> GetLastMessageOfConversationAsync(
            Guid conversationId
        )
        {
            var message = await GetAllAsync(
                m => new LastMessageProjection
                {
                    Id = m.Id,
                    ConversationId = m.ConversationId,
                    SenderId = m.SenderId,
                    Content = m.Content,
                    SequenceNumber = m.SequenceNumber,
                    CreatedAt = m.CreatedAt,
                    SenderFullName = m.Sender.FullName,
                    Media = m.Media!.Take(3).ToList(),
                    Sender = m.Sender,
                    Type = m.Type,
                },
                m => m.ConversationId == conversationId,
                m => m.SequenceNumber,
                true,
                1,
                null
            );

            return message.FirstOrDefault();
        }

        public async Task<List<Message>> GetMessages(
            Guid conversationId,
            int? cursor,
            bool desc,
            int limit
        )
        {
            return await GetAllAsync(
                m => m,
                m => m.ConversationId == conversationId,
                m => m.SequenceNumber,
                desc,
                limit,
                cursor == 0 ? int.MaxValue : cursor,
                m => m.Include(m => m.Sender).Include(m => m.Media!.Take(3))
            );
        }
    }
}

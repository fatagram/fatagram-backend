using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.Domain.Models;
using Fatagram.Infrastructure.Data;
using Fatagram.Infrastructure.Projections;
using Fatagram.Infrastructure.Repositories.BaseRepository;
using Fatagram.Infrastructure.Repositories.ConversationRepository.Interfaces;
using Fatagram.Shared.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Fatagram.Infrastructure.Repositories.ConversationRepository
{
    public class ConversationRepository : BaseRepository<Conversation>, IConversationRepository
    {
        public ConversationRepository(
            AppDbContext dbContext,
            ILogger<BaseRepository<Conversation>>? logger = null
        )
            : base(dbContext, logger) { }

        public async Task<List<ConversationProjection>> GetMyConversationsAsync(
            string userId,
            DateTime? cursor,
            int limit
        )
        {
            var query = _dbContext
                .Conversations.Where(c => c.Participants.Any(p => p.UserId == userId.ToGuid()))
                .Select(c => new ConversationProjection
                {
                    Id = c.Id,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt,
                    Participants = c.Participants,
                    LastMessage = c.Messages.OrderByDescending(m => m.CreatedAt).FirstOrDefault(),
                    UnreadMessagesCount = c.Messages.Count(m =>
                        m.CreatedAt > (cursor ?? DateTime.MinValue)
                        && m.SenderId != userId.ToGuid()
                        && m.ReadAt == DateTime.MinValue
                    ),
                    LastActiveAt =
                        c.Messages.OrderByDescending(m => m.CreatedAt)
                            .Select(m => (DateTime?)m.CreatedAt)
                            .FirstOrDefault()
                        ?? c.CreatedAt,
                });

            if (cursor.HasValue)
            {
                query = query.Where(c => c.LastActiveAt < cursor.Value);
            }

            return await query.OrderByDescending(c => c.LastActiveAt).Take(limit).ToListAsync();
        }
    }
}

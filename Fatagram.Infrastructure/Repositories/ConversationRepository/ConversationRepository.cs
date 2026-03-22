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

        public async Task<ConversationProjection?> GetConversationById(
            Guid userId,
            Guid conversationId
        )
        {
            return await _dbContext
                .Conversations.Where(c => c.Id == conversationId)
                .Select(c => new ConversationProjection
                {
                    Id = c.Id,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt,
                    ParticipantIds = c.Participants.Select(p => p.UserId).ToList(),
                    LastMessage = c.Messages.OrderByDescending(m => m.CreatedAt).FirstOrDefault(),
                    UnreadMessagesCount = c.Messages.Count(m => m.ReadAt == DateTime.MinValue),
                    LastActiveAt =
                        c.Messages.OrderByDescending(m => m.CreatedAt)
                            .Select(m => (DateTime?)m.CreatedAt)
                            .FirstOrDefault()
                        ?? c.CreatedAt,
                    Name = c.IsGroup
                        ? c.Name
                        : c
                            .Participants.Where(p => p.UserId != userId)
                            .Select(p => p.User!.FullName)
                            .FirstOrDefault(),
                    AvatarUrl = c.IsGroup
                        ? null
                        : c
                            .Participants.Where(p => p.UserId != userId)
                            .Select(p => p.User!.Avatar)
                            .FirstOrDefault(),
                })
                .FirstOrDefaultAsync();
        }

        public async Task<ConversationProjection?> GetConversationWith(
            Guid userId,
            Guid targetUserId
        )
        {
            var query = _dbContext
                .Conversations.Where(c =>
                    c.Participants.Any(p => p.UserId == userId)
                    && c.Participants.Any(p => p.UserId == targetUserId)
                    && !c.IsGroup
                )
                .Select(c => new ConversationProjection
                {
                    Id = c.Id,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt,
                    Participants = c.Participants,
                    LastMessage = c.Messages.OrderByDescending(m => m.CreatedAt).FirstOrDefault(),
                    UnreadMessagesCount = c.Messages.Count(m =>
                        m.SenderId != userId && m.ReadAt == DateTime.MinValue
                    ),
                    LastActiveAt =
                        c.Messages.OrderByDescending(m => m.CreatedAt)
                            .Select(m => (DateTime?)m.CreatedAt)
                            .FirstOrDefault()
                        ?? c.CreatedAt,
                    Name = c
                        .Participants.Where(p => p.UserId == targetUserId)
                        .Select(p => p.User!.FullName)
                        .FirstOrDefault(),
                    AvatarUrl = c
                        .Participants.Where(p => p.UserId == targetUserId)
                        .Select(p => p.User!.Avatar)
                        .FirstOrDefault(),
                });

            return await query.FirstOrDefaultAsync();
        }

        public async Task<List<ConversationProjection>> GetMyConversationsAsync(
            string userId,
            DateTime? cursor,
            int limit
        )
        {
            Console.WriteLine(
                $"[ConversationRepository] GetMyConversationsAsync: Fetching conversations for user {userId} with cursor {cursor} and limit {limit}"
            );
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
                    Name = c.IsGroup
                        ? c.Name
                        : c
                            .Participants.Where(p => p.UserId != userId.ToGuid())
                            .Select(p => p.User!.FullName)
                            .FirstOrDefault(),
                    AvatarUrl = c.IsGroup
                        ? c.AvatarUrl
                        : c
                            .Participants.Where(p => p.UserId != userId.ToGuid())
                            .Select(p => p.User!.Avatar)
                            .FirstOrDefault(),
                });

            if (cursor.HasValue && cursor.Value != DateTime.MinValue)
            {
                query = query.Where(c => c.LastActiveAt < cursor.Value);
            }

            return await query.OrderByDescending(c => c.LastActiveAt).Take(limit).ToListAsync();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
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
            Guid conversationId,
            Expression<Func<Conversation, ConversationProjection>>? selector = null
        )
        {
            var query = _dbContext.Conversations.Where(c => c.Id == conversationId);

            if (selector != null)
            {
                return await query.Select(selector).FirstOrDefaultAsync();
            }
            else
            {
                return await query
                    .Select(c => new ConversationProjection
                    {
                        Id = c.Id,
                        CreatedAt = c.CreatedAt,
                        UpdatedAt = c.UpdatedAt,
                        ParticipantIds = c.Participants.Select(p => p.UserId).ToList(),
                        LastMessage = c
                            .Messages.OrderByDescending(m => m.CreatedAt)
                            .Select(m => new LastMessageProjection
                            {
                                Id = m.Id,
                                ConversationId = m.ConversationId,
                                SenderId = m.SenderId,
                                Type = m.Type,
                                Metadata = m.Metadata,
                                Content = m.Content,
                                CreatedAt = m.CreatedAt,
                                SenderFullName = m.Sender.FullName,
                                SenderNickname = m
                                    .Sender.ConversationParticipants.Where(cp =>
                                        cp.ConversationId == c.Id && cp.UserId == m.SenderId
                                    )
                                    .Select(cp => cp.Nickname)
                                    .FirstOrDefault(),
                            })
                            .FirstOrDefault(),
                        IsGroup = c.IsGroup,
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
        }

        public Task<List<ConversationSeenInfoProjection>> GetConversationsSeenInfoAsync(Guid userId)
        {
            return _dbContext
                .ConversationParticipants.Where(cp => cp.UserId == userId)
                .Select(cp => new ConversationSeenInfoProjection
                {
                    ConversationId = cp.ConversationId,
                    LastMessageNumber = cp.Conversation.Messages.Any()
                        ? cp.Conversation.Messages.Max(m => m.SequenceNumber)
                        : cp.Conversation.LastMessageNumber,
                    UserLastSeenMessageNumber = cp.LastSeenNumber,
                })
                .ToListAsync();
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
                    LastMessage = c
                        .Messages.OrderByDescending(m => m.CreatedAt)
                        .Select(m => new LastMessageProjection
                        {
                            Id = m.Id,
                            ConversationId = m.ConversationId,
                            SenderId = m.SenderId,
                            Type = m.Type,
                            Metadata = m.Metadata,
                            Content = m.Content,
                            CreatedAt = m.CreatedAt,
                            SenderFullName = m.Sender.FullName,
                            SenderNickname = m
                                .Sender.ConversationParticipants.Where(cp =>
                                    cp.ConversationId == c.Id && cp.UserId == m.SenderId
                                )
                                .Select(cp => cp.Nickname)
                                .FirstOrDefault(),
                        })
                        .FirstOrDefault(),
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
                    LastMessage = c
                        .Messages.OrderByDescending(m => m.CreatedAt)
                        .Select(m => new LastMessageProjection
                        {
                            Id = m.Id,
                            ConversationId = m.ConversationId,
                            SenderId = m.SenderId,
                            Type = m.Type,
                            Metadata = m.Metadata,
                            Content = m.Content,
                            CreatedAt = m.CreatedAt,
                            SenderFullName = m.Sender.FullName,
                            SenderNickname = m
                                .Sender.ConversationParticipants.Where(cp =>
                                    cp.ConversationId == c.Id && cp.UserId == m.SenderId
                                )
                                .Select(cp => cp.Nickname)
                                .FirstOrDefault(),
                        })
                        .FirstOrDefault(),
                    LastMessageNumber = c.LastMessageNumber,
                    IsGroup = c.IsGroup,
                    TopParticipantNames = c.IsGroup
                        ? c
                            .Participants.OrderBy(p => p.CreatedAt)
                            .Select(p => p.User!.FullName!)
                            .Take(2)
                            .ToList()
                        : null,
                    OtherUserId = c.IsGroup
                        ? null
                        : c
                            .Participants.Where(p => p.UserId != userId.ToGuid())
                            .Select(p => p.UserId)
                            .FirstOrDefault(),
                    ParticipantCount = c.IsGroup ? c.Participants.Count() : null,
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

        public async Task<int> GetUnreadCountAsync(Guid userId)
        {
            var count =
                await _dbContext
                    .ConversationParticipants.Where(cp => cp.UserId == userId)
                    .Select(cp => (int?)(cp.Conversation.LastMessageNumber - cp.LastSeenNumber))
                    .SumAsync() ?? 0;
            return count;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using Fatagram.Application.Dtos.Conversation;
using Fatagram.Application.Dtos.Filter;
using Fatagram.Application.Dtos.Message;
using Fatagram.Application.Exceptions;
using Fatagram.Application.Exceptions.MiddleLevelExceptions;
using Fatagram.Application.Services.ConversationServices.Interfaces;
using Fatagram.Application.Services.MessageServices.Interfaces;
using Fatagram.Application.Utils;
using Fatagram.Domain.Enums;
using Fatagram.Domain.Models;
using Fatagram.Infrastructure.Cache;
using Fatagram.Infrastructure.Projections;
using Fatagram.Infrastructure.Repositories.ConversationParticipantRepository.Interfaces;
using Fatagram.Infrastructure.Repositories.ConversationRepository.Interfaces;
using Fatagram.Infrastructure.Repositories.UserRepository.Interface;
using Fatagram.Shared.Common;
using Fatagram.Shared.Enums;
using Fatagram.Shared.Extensions;

namespace Fatagram.Application.Services.ConversationServices
{
    public class ConversationService(
        IConversationRepository conversationRepository,
        IConversationParticipantRepository conversationParticipantRepository,
        IUserRepository userRepository,
        IMessageService messageService,
        ICacheService cacheService,
        IMapper mapper,
        Fatagram.Infrastructure.Repositories.MessageRepository.Interfaces.IMessageRepository messageRepository
    ) : IConversationService
    {
        private readonly IConversationRepository _conversationRepository = conversationRepository;
        private readonly IConversationParticipantRepository _conversationParticipantRepository =
            conversationParticipantRepository;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IMessageService _messageService = messageService;
        private readonly Fatagram.Infrastructure.Repositories.MessageRepository.Interfaces.IMessageRepository _messageRepository =
            messageRepository;
        private readonly ICacheService _cacheService = cacheService;
        private readonly IMapper _mapper = mapper;

        public async Task<Result<CursorResult<ConversationDto, DateTime>>> GetAllAsync(
            Guid userId,
            CursorFilter<DateTime>? cursor = null
        )
        {
            var conservations = await _conversationRepository.GetMyConversationsAsync(
                userId.ToString(),
                cursor?.Cursor,
                cursor?.Limit ?? 20
            );
            if (conservations.Count == 0)
            {
                return Result<CursorResult<ConversationDto, DateTime>>.Create(
                    ResponseStatusCode.Success,
                    new CursorResult<ConversationDto, DateTime>
                    {
                        Items = new List<ConversationDto>(),
                        NextCursor = null,
                        HasNext = false,
                    }
                );
            }
            var res = _mapper.Map<List<ConversationDto>>(conservations);

            var convIds = res.Select(c => c.Id).ToList();
            var otherUserIds = res.Where(c => !c.IsGroup && c.OtherUserId != null)
                .Select(c => c.OtherUserId!.Value)
                .ToHashSet();

            var userLastReadMapTask = _cacheService.HashGetAllAsync($"user:{userId}:last_read");

            var maxSeqMap = convIds.Select(id =>
                _cacheService.HashGetAsync($"conv:{id}:meta", "max_seq")
            );
            var maxSeqResultsTask = Task.WhenAll(maxSeqMap);

            var seenTasks = convIds.Select(id => _cacheService.HashGetAllAsync($"conv:{id}:seen"));
            var seenResultsTask = Task.WhenAll(seenTasks);

            var dbParticipantsTask = _conversationParticipantRepository.GetAllAsync(
                cp => convIds.Contains(cp.ConversationId.ToString()),
                cp => new
                {
                    cp.ConversationId,
                    cp.UserId,
                    cp.LastSeenNumber,
                    cp.LastSeenMessageId,
                    cp.SeenAt,
                }
            );

            await Task.WhenAll(
                userLastReadMapTask,
                maxSeqResultsTask,
                seenResultsTask,
                dbParticipantsTask
            );

            var userLastReadMap = await userLastReadMapTask;
            var maxSeqResults = await maxSeqResultsTask;
            var seenResults = await seenResultsTask;
            var dbParticipants = await dbParticipantsTask;

            var redisMaxSeqMap = convIds
                .Select((id, index) => new { id, val = maxSeqResults[index] })
                .ToDictionary(x => x.id, x => int.TryParse(x.val, out var n) ? n : (int?)null);

            // Map SeenInfo từ Redis & DB (Redis đè DB)
            var finalSeenMap = dbParticipants.ToDictionary(
                p => (p.ConversationId, p.UserId),
                p => new ParticipantSeenInfo
                {
                    MessageId = p.LastSeenMessageId ?? Guid.Empty,
                    SeenAt = p.SeenAt ?? DateTime.MinValue,
                    SequenceNumber = p.LastSeenNumber,
                }
            );

            for (int i = 0; i < convIds.Count; i++)
            {
                var convId = convIds[i];
                foreach (var entry in seenResults[i]) // entry: userId -> json
                {
                    if (Guid.TryParse(entry.Key, out var uId))
                    {
                        var redisSeen = JsonSerializer.Deserialize<ParticipantSeenInfo>(
                            entry.Value
                        );
                        if (redisSeen != null)
                            finalSeenMap[(convId.ToGuid(), uId)] = redisSeen;
                    }
                }
            }

            foreach (var conversation in res)
            {
                var cGuid = conversation.Id.ToGuid();
                int lastRead =
                    userLastReadMap.TryGetValue(cGuid.ToString(), out var lastReadStr)
                    && int.TryParse(lastReadStr, out var lr)
                        ? lr
                        : 0;
                if (
                    lastRead == 0
                    && finalSeenMap.TryGetValue(
                        (conversation.Id.ToGuid(), userId),
                        out var seenInfo
                    )
                )
                {
                    lastRead = seenInfo.SequenceNumber;
                }

                int maxSeq =
                    redisMaxSeqMap.TryGetValue(cGuid.ToString(), out var mSeq) && mSeq.HasValue
                        ? mSeq.Value
                        : conversation.LastMessageNumber;

                conversation.UnreadMessageCount = Math.Max(0, maxSeq - lastRead);

                if (finalSeenMap.TryGetValue((cGuid, userId), out var me))
                    conversation.MyLastSeenMessageId = me.MessageId;

                if (!conversation.IsGroup && conversation.OtherUserId.HasValue)
                {
                    if (
                        finalSeenMap.TryGetValue(
                            (cGuid, conversation.OtherUserId.Value),
                            out var other
                        )
                    )
                        conversation.OtherLastSeenMessageId = other.MessageId;
                }
            }

            return Result<CursorResult<ConversationDto, DateTime>>.Create(
                ResponseStatusCode.Success,
                new CursorResult<ConversationDto, DateTime>
                {
                    Items = res,
                    NextCursor = conservations.Count > 0 ? conservations.Last().LastActiveAt : null,
                    HasNext = conservations.Count == (cursor?.Limit ?? 20),
                }
            );
        }

        public async Task<Result<ConversationDto>> GetByIdAsync(Guid userId, Guid conversationId)
        {
            var conversation = await _conversationRepository.GetConversationById(
                userId,
                conversationId,
                c => new ConversationProjection
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
                            Content = m.Content,
                            CreatedAt = m.CreatedAt,
                            SenderFullName = m.Sender.FullName!,
                            SenderNickname = m
                                .Sender.ConversationParticipants.Where(cp =>
                                    cp.ConversationId == c.Id && cp.UserId == m.SenderId
                                )
                                .Select(cp => cp.Nickname)
                                .FirstOrDefault(),
                        })
                        .FirstOrDefault(),
                    UnreadMessageCount = c.Messages.Count(m =>
                        m.SenderId != userId && m.ReadAt == DateTime.MinValue
                    ),
                    IsGroup = c.IsGroup,
                    LastActiveAt =
                        c.Messages.OrderByDescending(m => m.CreatedAt)
                            .Select(m => (DateTime?)m.CreatedAt)
                            .FirstOrDefault()
                        ?? c.CreatedAt,
                    TopParticipantNames =
                        c.IsGroup && c.Name == null
                            ? c
                                .Participants.OrderBy(p => p.CreatedAt)
                                .Select(p => p.User!.FullName!)
                                .Take(2)
                                .ToList()
                            : null,
                    ParticipantCount = c.IsGroup ? c.Participants.Count() : null,
                    Name = c.IsGroup
                        ? c.Name
                        : c
                            .Participants.Where(p => p.UserId != userId)
                            .Select(p => p.User!.FullName)
                            .FirstOrDefault(),
                    AvatarUrl = c.IsGroup
                        ? c.AvatarUrl
                        : c
                            .Participants.Where(p => p.UserId != userId)
                            .Select(p => p.User!.Avatar)
                            .FirstOrDefault(),
                }
            );
            if (conversation == null)
            {
                throw new NotFoundException(
                    new Error("CONVERSATION_NOT_FOUND", "Conversation not found")
                );
            }
            return Result<ConversationDto>.Create(
                ResponseStatusCode.Success,
                _mapper.Map<ConversationDto>(conversation)
            );
        }

        public async Task<Result<ConversationDto>> GetWithAsync(Guid userId, Guid targetUserId)
        {
            var conversation = await _conversationRepository.GetConversationWith(
                userId,
                targetUserId
            );

            if (conversation == null)
            {
                throw new NotFoundException(
                    new Error("CONVERSATION_NOT_FOUND", "Conversation not found")
                );
            }

            return Result<ConversationDto>.Create(
                ResponseStatusCode.Success,
                _mapper.Map<ConversationDto>(conversation)
            );
        }

        public Task<Result<ConversationDto>> CreateAsync(Guid creatorId, string conversationType)
        {
            throw new NotImplementedException();
        }

        public Task<Result<ConversationDto>> CreateAsync(Guid creatorId, Guid otherUserId)
        {
            throw new NotImplementedException();
        }

        public Task<Result<ConversationDto>> CreateConversationAsync(
            Guid creatorId,
            string conversationType
        )
        {
            throw new NotImplementedException();
        }

        public async Task<Result<Guid>> CreateGroupAsync(
            Guid creatorId,
            IEnumerable<Guid> participantIds,
            string? name
        )
        {
            var allUserIds = participantIds.Append(creatorId).ToHashSet();
            if (allUserIds.Count < 3)
            {
                throw new AppException(
                    new Error(
                        "INVALID_PARTICIPANTS",
                        "Group conversation must have at least 3 unique participants (including creator)"
                    )
                );
            }
            var conversation = new Conversation
            {
                IsGroup = true,
                Name = name,
                Participants = participantIds
                    .Select(id => new ConversationParticipant { UserId = id })
                    .Append(
                        new ConversationParticipant
                        {
                            UserId = creatorId,
                            Role = ConversationRole.Owner,
                        }
                    )
                    .ToList(),
            };
            var res = await _conversationRepository.AddAsync(conversation);
            var creatorFullName = await _userRepository.GetByUniqueAsync(
                u => u.Id == creatorId,
                u => u.FullName
            );
            if (creatorFullName == null)
                throw new AppException(new Error("CREATOR_NOT_FOUND", "Creator user not found"));

            await _messageService.SendMessageAsync(
                null,
                new CreateMessageRequest
                {
                    ConversationId = res.Id,
                    Type = MessageType.CreateGroup,
                    Metadata = new Dictionary<string, object>
                    {
                        { "creatorId", creatorId.ToString() },
                        { "creatorName", creatorFullName! },
                    },
                }
            );

            await _cacheService.SetAsync(
                $"conversation:{res.Id}:participants",
                participantIds
                    .Select(id => new ParticipantDto { UserId = id, CreatedAt = DateTime.UtcNow })
                    .Append(new ParticipantDto { UserId = creatorId, CreatedAt = DateTime.UtcNow })
                    .ToList(),
                TimeSpan.FromDays(2)
            );
            return Result<Guid>.Create(ResponseStatusCode.Created, res.Id);
        }

        public async Task<Result<int>> GetUnreadCountAsync(Guid userId)
        {
            var userLastReadMapTask = _cacheService.HashGetAllAsync($"user:{userId}:last_read");

            var dbConversations = await _conversationRepository.GetConversationsSeenInfoAsync(
                userId
            );

            if (!dbConversations.Any())
                return Result<int>.Create(ResponseStatusCode.Success, 0);

            var convIds = dbConversations.Select(c => c.ConversationId).ToList();
            var maxSeqTasks = convIds.Select(id =>
                _cacheService.HashGetAsync($"conv:{id}:meta", "max_seq")
            );

            await Task.WhenAll(userLastReadMapTask, Task.WhenAll(maxSeqTasks));

            var userLastReadMap = await userLastReadMapTask;
            var maxSeqResults = await Task.WhenAll(maxSeqTasks);

            int totalUnreadCount = 0;

            for (int i = 0; i < dbConversations.Count; i++)
            {
                var conv = dbConversations[i];
                var convIdStr = conv.ConversationId.ToString();

                var maxSeq = conv.LastMessageNumber;
                if (int.TryParse(maxSeqResults[i], out var cachedMaxSeq))
                {
                    maxSeq = Math.Max(maxSeq, cachedMaxSeq);
                }

                var lastSeenSeq = conv.UserLastSeenMessageNumber;
                if (
                    userLastReadMap.TryGetValue(convIdStr, out var cacheLastReadStr)
                    && int.TryParse(cacheLastReadStr, out var cacheLastRead)
                )
                {
                    lastSeenSeq = Math.Max(lastSeenSeq, cacheLastRead);
                }

                if (maxSeq > lastSeenSeq)
                {
                    totalUnreadCount++;
                }
            }

            return Result<int>.Create(ResponseStatusCode.Success, totalUnreadCount);
        }
    }
}

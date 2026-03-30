using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
        IMapper mapper
    ) : IConversationService
    {
        private readonly IConversationRepository _conversationRepository = conversationRepository;
        private readonly IConversationParticipantRepository _conversationParticipantRepository =
            conversationParticipantRepository;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IMessageService _messageService = messageService;
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
            var res = _mapper.Map<List<ConversationDto>>(conservations);

            foreach (var conversation in res)
            {
                var seenCacheKey = $"conv:{conversation.Id}:seen";
                if (!conversation.IsGroup && conversation?.OtherUserId != null)
                {
                    var otherLastSeen = await _cacheService.HashGetAsync(
                        seenCacheKey,
                        conversation.OtherUserId.ToString() ?? ""
                    );
                    conversation.OtherLastSeenMessageId = otherLastSeen.ToGuid();
                }

                var lastSeen = await _cacheService.HashGetAsync(seenCacheKey, userId.ToString());
                if (conversation != null && lastSeen != null)
                {
                    conversation.MyLastSeenMessageId = lastSeen.ToGuid();
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
                    UnreadMessagesCount = c.Messages.Count(m =>
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
                            .Select(p => p.User.FullName)
                            .FirstOrDefault(),
                    AvatarUrl = c.IsGroup
                        ? c.AvatarUrl
                        : c
                            .Participants.Where(p => p.UserId != userId)
                            .Select(p => p.User.Avatar)
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
    }
}

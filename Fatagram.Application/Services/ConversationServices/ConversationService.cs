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
using Fatagram.Infrastructure.Repositories.MessageRepository.Interfaces;
using Fatagram.Infrastructure.Repositories.UserRepository.Interface;
using Fatagram.Shared.Common;
using Fatagram.Shared.Enums;
using Fatagram.Shared.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Fatagram.Application.Services.ConversationServices
{
    public class ConversationService(
        IConversationRepository conversationRepository,
        IConversationParticipantRepository conversationParticipantRepository,
        IUserRepository userRepository,
        IMessageRepository messageRepository,
        IMessageService messageService,
        ICacheService cacheService,
        IMapper mapper
    ) : IConversationService
    {
        private readonly IConversationRepository _conversationRepository = conversationRepository;
        private readonly IConversationParticipantRepository _cpRepo =
            conversationParticipantRepository;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IMessageRepository _messageRepository = messageRepository;
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
            if (conservations.Count == 0)
            {
                return Result<CursorResult<ConversationDto, DateTime>>.Create(
                    ResponseStatusCode.Success,
                    new CursorResult<ConversationDto, DateTime>
                    {
                        Items = [],
                        NextCursor = null,
                        HasNext = false,
                    }
                );
            }
            var res = _mapper.Map<List<ConversationDto>>(conservations);

            foreach (var c in res)
            {
                var participantSeenInfos = await _cpRepo.GetConversationParticipantsSeenInfoAsync(
                    c.Id.ToGuid()
                );
                c.MyLastSeenMessageSeq = participantSeenInfos
                    .ParticipantsSeenInfo[userId]
                    .SequenceNumber;
                if (!c.IsGroup && c.OtherUserId != null)
                {
                    c.OtherLastSeenMessageSeq = participantSeenInfos
                        .ParticipantsSeenInfo[c.OtherUserId ?? Guid.Empty]
                        .SequenceNumber;
                }

                var lastM = await _messageRepository.GetLastMessageOfConversationAsync(
                    c.Id.ToGuid()
                );
                c.LastMessage = _mapper.Map<ResponseMessageDto>(lastM);
                c.LastMessageNumber = c.LastMessage?.SequenceNumber ?? 0;
            }

            return Result<CursorResult<ConversationDto, DateTime>>.Create(
                ResponseStatusCode.Success,
                new CursorResult<ConversationDto, DateTime>
                {
                    Items = res,
                    NextCursor = conservations.Last().LastActiveAt,
                    HasNext = conservations.Count == (cursor?.Limit ?? 20),
                }
            );
        }

        public async Task<Result<List<ConversationDto>>> GetDeltaAsync(Guid userId, DateTime since)
        {
            var conversations = await _conversationRepository.GetDeltaAsync(userId, since);

            if (conversations.Count == 0)
            {
                return Result<List<ConversationDto>>.Create(ResponseStatusCode.Success, []);
            }

            var res = _mapper.Map<List<ConversationDto>>(conversations);

            foreach (var c in res)
            {
                var participantSeenInfos = await _cpRepo.GetConversationParticipantsSeenInfoAsync(
                    c.Id.ToGuid()
                );
                c.MyLastSeenMessageSeq = participantSeenInfos
                    .ParticipantsSeenInfo[userId]
                    .SequenceNumber;
                if (!c.IsGroup && c.OtherUserId != null)
                {
                    c.OtherLastSeenMessageSeq = participantSeenInfos
                        .ParticipantsSeenInfo[c.OtherUserId ?? Guid.Empty]
                        .SequenceNumber;
                }

                var lastM = await _messageRepository.GetLastMessageOfConversationAsync(
                    c.Id.ToGuid()
                );
                c.LastMessage = _mapper.Map<ResponseMessageDto>(lastM);
                c.LastMessageNumber = c.LastMessage?.SequenceNumber ?? 0;
            }

            return Result<List<ConversationDto>>.Create(ResponseStatusCode.Success, res);
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
                    LastMessageNumber = c.LastMessageNumber,
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
                            : null!,
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

            var convSeq = await _cacheService.GetAsync<int>($"conv:{conversationId}:seq");
            if (convSeq > conversation.LastMessageNumber)
            {
                conversation.LastMessageNumber = convSeq;
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
                Participants =
                [
                    .. participantIds
                        .Select(id => new ConversationParticipant { UserId = id })
                        .Append(
                            new ConversationParticipant
                            {
                                UserId = creatorId,
                                Role = ConversationRole.Owner,
                            }
                        ),
                ],
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

            return Result<Guid>.Create(ResponseStatusCode.Created, res.Id);
        }

        public async Task<Result<int>> GetUnreadCountAsync(Guid userId)
        {
            return Result<int>.Create(
                ResponseStatusCode.Success,
                await _conversationRepository.GetUnreadCountAsync(userId)
            );
        }

        public async Task<Result> UpdateAsync(
            Guid conversationId,
            UpdateConversationDto conversationDto
        )
        {
            var existingConversation =
                await _conversationRepository.GetAsync(conversationId, c => c)
                ?? throw new NotFoundException(
                    new Error("CONVERSATION_NOT_FOUND", "Conversation not found")
                );

            _mapper.Map(conversationDto, existingConversation);
            await _conversationRepository.UpdateAsync(
                existingConversation.NormalizeEmptyStringToNull()
            );
            return Result.Create(ResponseStatusCode.Success);
        }

        public async Task<Result> UpdateAvatarAsync(
            Guid conversationId,
            string avatarUrl,
            Guid userId
        )
        {
            Console.WriteLine(
                $"[ConversationService] UpdateAvatarAsync: conversationId={conversationId}, avatarUrl={avatarUrl}, userId={userId}"
            );
            var existingConversation =
                await _conversationRepository.GetAsync(conversationId, c => c)
                ?? throw new NotFoundException(
                    new Error("CONVERSATION_NOT_FOUND", "Conversation not found")
                );

            var user =
                await _userRepository.GetAsync(userId, u => u)
                ?? throw new NotFoundException(new Error("USER_NOT_FOUND", "User not found"));

            existingConversation.AvatarUrl = avatarUrl;
            await _conversationRepository.UpdateAsync(existingConversation);

            // Send system message for avatar update
            var message = await _messageService.SendMessageAsync(
                null,
                new CreateMessageRequest
                {
                    ConversationId = conversationId,
                    Content = $"{user.FullName} đã cập nhật avatar",
                    Type = MessageType.ChangeGroupAvatar,
                    Metadata = new Dictionary<string, object>
                    {
                        { "avatarUrl", avatarUrl },
                        { "actorName", user.FullName! },
                        { "actorId", userId },
                    },
                }
            );

            return Result.Create(ResponseStatusCode.Success);
        }
    }
}

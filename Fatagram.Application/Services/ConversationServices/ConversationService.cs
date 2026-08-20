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
using Fatagram.Application.Services.ConversationServices;
using Fatagram.Application.Services.MessageServices;
using Fatagram.Application.Utils;
using Fatagram.Domain.Enums;
using Fatagram.Domain.Models;
using Fatagram.Application.Abstractions.Cache;
using Fatagram.Application.Common.Projections;
using Fatagram.Application.Abstractions.Repositories;
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
        IPermissionRepository permissionRepository,
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
        private readonly IPermissionRepository _permissionRepository = permissionRepository;
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
                await PopulateConversationMetaAsync(userId, c);
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
                await PopulateConversationMetaAsync(userId, c);
            }

            return Result<List<ConversationDto>>.Create(ResponseStatusCode.Success, res);
        }

        public async Task<Result<ConversationDto>> GetByIdAsync(Guid userId, Guid conversationId)
        {
            var conversation =
                await _conversationRepository.GetConversationById(
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
                                .Participants.OrderByDescending(p => p.UserId != userId)
                                .Select(p => p.User!.FullName)
                                .FirstOrDefault(),
                        AvatarUrl = c.IsGroup
                            ? c.AvatarUrl
                            : c
                                .Participants.OrderByDescending(p => p.UserId != userId)
                                .Select(p => p.User!.Avatar)
                                .FirstOrDefault(),
                        OtherUserId = c.IsGroup
                            ? null
                            : c.Participants.OrderByDescending(p => p.UserId != userId)
                                .Select(p => (Guid?)p.UserId)
                                .FirstOrDefault()
                            ?? userId,
                        BackgroundUrl = c.BackgroundUrl,
                        Theme = c.Theme,
                        PinnedAt = c.Participants
                            .Where(p => p.UserId == userId)
                            .Select(p => p.PinnedAt)
                            .FirstOrDefault(),
                        IsPinned = c.Participants.Any(p => p.UserId == userId && p.PinnedAt != null),
                    }
                )
                ?? throw new NotFoundException(
                    new Error("CONVERSATION_NOT_FOUND", "Conversation not found")
                );

            var dto = _mapper.Map<ConversationDto>(conversation);
            await PopulateConversationMetaAsync(userId, dto);

            return Result<ConversationDto>.Create(ResponseStatusCode.Success, dto);
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

            var dto = _mapper.Map<ConversationDto>(conversation);
            await PopulateConversationMetaAsync(userId, dto);

            return Result<ConversationDto>.Create(ResponseStatusCode.Success, dto);
        }

        private async Task PopulateConversationMetaAsync(Guid userId, ConversationDto dto)
        {
            var convId = dto.Id.ToGuid();
            var participant = await _cpRepo.GetByUniqueAsync(
                cp => cp.ConversationId == convId && cp.UserId == userId,
                cp => cp
            );

            if (dto.MyLastSeenMessageSeq == null && participant != null)
            {
                dto.MyLastSeenMessageSeq = participant.LastSeenNumber;
            }

            var lastSeq = dto.LastMessageNumber > 0 ? dto.LastMessageNumber : (dto.LastMessage?.SequenceNumber ?? 0);
            var mySeenSeq = dto.MyLastSeenMessageSeq ?? participant?.LastSeenNumber ?? 0;
            dto.UnreadMessageCount = Math.Max(0, lastSeq - mySeenSeq);

            dto.MyRole = participant?.Role ?? (dto.IsGroup ? ConversationRole.Member : ConversationRole.Owner);

            var perms = (await _permissionRepository.GetPermissionNamesAsync(userId, convId)).ToHashSet();

            if (!dto.IsGroup)
            {
                dto.Capabilities = new ConversationCapabilitiesDto
                {
                    CanSendMessage = true,
                    CanChangeAvatar = false,
                    CanChangeName = false,
                    CanChangeTheme = true,
                    CanChangeBackground = true,
                    CanKickMember = false,
                    CanAddMember = false,
                    CanPinMessage = true,
                    CanDeleteConversation = false,
                };
            }
            else
            {
                var isOwner = dto.MyRole == ConversationRole.Owner || perms.Contains("conversation.owner");
                var isAdmin = isOwner || dto.MyRole == ConversationRole.Admin || perms.Contains("conversation.admin");

                dto.Capabilities = new ConversationCapabilitiesDto
                {
                    CanSendMessage = perms.Contains("conversation.send_message") || perms.Contains("conversation.member") || isAdmin,
                    CanChangeAvatar = perms.Contains("conversation.settings.update") || isAdmin,
                    CanChangeName = perms.Contains("conversation.settings.update") || isAdmin,
                    CanChangeTheme = perms.Contains("conversation.settings.update") || isAdmin,
                    CanChangeBackground = perms.Contains("conversation.settings.update") || isAdmin,
                    CanKickMember = perms.Contains("conversation.member.kick") || perms.Contains("conversation.member.manage") || isAdmin,
                    CanAddMember = perms.Contains("conversation.member.manage") || isAdmin,
                    CanPinMessage = perms.Contains("conversation.message.pin") || isAdmin,
                    CanDeleteConversation = perms.Contains("conversation.delete") || isOwner,
                };
            }
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
            var now = DateTime.UtcNow;
            var participants = participantIds
                .Where(id => id != creatorId)
                .Distinct()
                .Select(id => new ConversationParticipant
                {
                    UserId = id,
                    Role = ConversationRole.Member,
                    CreatedAt = now,
                })
                .Append(
                    new ConversationParticipant
                    {
                        UserId = creatorId,
                        Role = ConversationRole.Owner,
                        CreatedAt = now,
                    }
                )
                .ToList();

            var conversation = new Conversation
            {
                IsGroup = true,
                Name = name,
                Participants = participants,
            };
            var res = await _conversationRepository.AddAsync(conversation);
            var creatorFullName =
                await _userRepository.GetByUniqueAsync(u => u.Id == creatorId, u => u.FullName)
                ?? throw new AppException(new Error("CREATOR_NOT_FOUND", "Creator user not found"));

            await _messageService.SendMessageAsync(
                creatorId,
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

        public async Task<Result<CursorResult<ParticipantDto, DateTime>>> GetParticipantsAsync(
            Guid userId,
            Guid conversationId,
            CursorFilter<DateTime> filter
        )
        {
            var participants = await _cpRepo.GetParticipantsAsync(
                conversationId,
                filter.Cursor,
                filter.Limit
            );

            var items = participants
                .Select(p => new ParticipantDto
                {
                    UserId = p.UserId,
                    Fullname = p.User!.FullName!,
                    AvatarUrl = p.User!.Avatar,
                    Nickname = p.Nickname,
                    Role = p.Role,
                    CreatedAt = p.CreatedAt,
                })
                .ToList();

            return Result<CursorResult<ParticipantDto, DateTime>>.Create(
                ResponseStatusCode.Success,
                new CursorResult<ParticipantDto, DateTime>
                {
                    Items = items,
                    NextCursor = items.Count > 0 ? items.Last().CreatedAt : null,
                    HasNext = items.Count == filter.Limit,
                }
            );
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

            if (!existingConversation.IsGroup)
            {
                throw new AppException(
                    new Error(
                        "INVALID_OPERATION",
                        "Only group conversations can have their avatar updated"
                    )
                );
            }

            var user =
                await _userRepository.GetAsync(userId, u => u)
                ?? throw new NotFoundException(new Error("USER_NOT_FOUND", "User not found"));

            existingConversation.AvatarUrl = avatarUrl;
            await _conversationRepository.UpdateAsync(existingConversation);

            // Send system message for avatar update
            var message = await _messageService.SendMessageAsync(
                userId,
                new CreateMessageRequest
                {
                    ConversationId = conversationId,
                    Content = $"{user.FullName} Ä‘Ã£ cáº­p nháº­t avatar",
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

        public async Task<Result> UpdateNameAsync(Guid conversationId, string name, Guid userId)
        {
            var existingConversation =
                await _conversationRepository.GetByUniqueAsync(
                    c => c.Id == conversationId,
                    c => c,
                    q => q.Include(c => c.Participants).ThenInclude(p => p.User)
                )
                ?? throw new NotFoundException(
                    new Error("CONVERSATION_NOT_FOUND", "Conversation not found")
                );

            if (!existingConversation.IsGroup)
            {
                throw new AppException(
                    new Error(
                        "INVALID_OPERATION",
                        "Only group conversations can have their name updated"
                    )
                );
            }

            var user =
                await _userRepository.GetAsync(userId, u => u)
                ?? throw new NotFoundException(new Error("USER_NOT_FOUND", "User not found"));

            existingConversation.Name = name;
            UpdateSearchTextInternal(existingConversation);
            await _conversationRepository.UpdateAsync(existingConversation);

            await _messageService.SendMessageAsync(
                userId,
                new CreateMessageRequest
                {
                    ConversationId = conversationId,
                    Content = $"{user.FullName} Ä‘Ã£ Ä‘á»•i tÃªn nhÃ³m thÃ nh {name}",
                    Type = MessageType.RenameGroup,
                    Metadata = new Dictionary<string, object>
                    {
                        { "newName", name },
                        { "actorName", user.FullName! },
                        { "actorId", userId },
                    },
                }
            );

            return Result.Create(ResponseStatusCode.Success);
        }

        public async Task<Result> UpdateBackgroundUrlAsync(
            Guid conversationId,
            string backgroundUrl,
            Guid userId
        )
        {
            var existingConversation =
                await _conversationRepository.GetAsync(conversationId, c => c)
                ?? throw new NotFoundException(
                    new Error("CONVERSATION_NOT_FOUND", "Conversation not found")
                );

            var user =
                await _userRepository.GetAsync(userId, u => u)
                ?? throw new NotFoundException(new Error("USER_NOT_FOUND", "User not found"));

            existingConversation.BackgroundUrl = backgroundUrl;
            await _conversationRepository.UpdateAsync(existingConversation);

            await _messageService.SendMessageAsync(
                userId,
                new CreateMessageRequest
                {
                    ConversationId = conversationId,
                    Content = $"{user.FullName} Ä‘Ã£ cáº­p nháº­t hÃ¬nh ná»n cuá»™c trÃ² chuyá»‡n",
                    Type = MessageType.ChangeBackgroundUrl,
                    Metadata = new Dictionary<string, object>
                    {
                        { "backgroundUrl", backgroundUrl },
                        { "actorName", user.FullName! },
                        { "actorId", userId },
                    },
                }
            );

            return Result.Create(ResponseStatusCode.Success);
        }

        public async Task<Result> UpdateThemeAsync(Guid conversationId, string theme, Guid userId)
        {
            var existingConversation =
                await _conversationRepository.GetAsync(conversationId, c => c)
                ?? throw new NotFoundException(
                    new Error("CONVERSATION_NOT_FOUND", "Conversation not found")
                );

            var user =
                await _userRepository.GetAsync(userId, u => u)
                ?? throw new NotFoundException(new Error("USER_NOT_FOUND", "User not found"));

            existingConversation.Theme = theme;
            await _conversationRepository.UpdateAsync(existingConversation);

            await _messageService.SendMessageAsync(
                userId,
                new CreateMessageRequest
                {
                    ConversationId = conversationId,
                    Content = $"{user.FullName} Ä‘Ã£ Ä‘á»•i chá»§ Ä‘á» cuá»™c trÃ² chuyá»‡n",
                    Type = MessageType.ChangeTheme,
                    Metadata = new Dictionary<string, object>
                    {
                        { "theme", theme },
                        { "actorName", user.FullName! },
                        { "actorId", userId },
                    },
                }
            );

            return Result.Create(ResponseStatusCode.Success);
        }

        public async Task<Result<CursorResult<ConversationDto, Guid>>> SearchAsync(
            Guid userId,
            CursorFilter<Guid> filter
        )
        {
            var conversations = await _conversationRepository.SearchConversations(
                userId,
                filter.Keyword ?? "",
                filter.Limit,
                filter.Cursor
            );

            if (conversations.Count == 0)
            {
                return Result<CursorResult<ConversationDto, Guid>>.Create(
                    ResponseStatusCode.Success,
                    new CursorResult<ConversationDto, Guid>
                    {
                        Items = [],
                        NextCursor = null,
                        HasNext = false,
                    }
                );
            }

            var res = _mapper.Map<List<ConversationDto>>(conversations);

            return Result<CursorResult<ConversationDto, Guid>>.Create(
                ResponseStatusCode.Success,
                new CursorResult<ConversationDto, Guid>
                {
                    Items = res,
                    NextCursor = conversations.Last().Id,
                    HasNext = conversations.Count == filter.Limit,
                }
            );
        }

        private static void UpdateSearchTextInternal(Conversation conversation)
        {
            var name = conversation.Name ?? "";
            var participantsInfo = string.Join(
                " ",
                conversation.Participants.Select(p =>
                    $"{p.User?.FullName ?? ""} {p.Nickname ?? ""}"
                )
            );

            var rawText = $"{participantsInfo} {name}".Trim();
            conversation.SearchText = rawText.RemoveVietnameseTone().ToLowerInvariant();
        }

        public async Task<Result<bool>> TogglePinAsync(Guid conversationId, Guid userId)
        {
            var isPinned = await _cpRepo.TogglePinAsync(conversationId, userId);
            return Result<bool>.Create(ResponseStatusCode.Success, isPinned);
        }

        public async Task<Result> AddParticipantsAsync(
            Guid conversationId,
            IEnumerable<Guid> participantIds,
            Guid actorId
        )
        {
            var conversation = await _conversationRepository.GetByUniqueAsync(
                c => c.Id == conversationId,
                c => c,
                q => q.Include(c => c.Participants).ThenInclude(p => p.User)
            ) ?? throw new NotFoundException(new Error("CONVERSATION_NOT_FOUND", "Conversation not found"));

            if (!conversation.IsGroup)
            {
                throw new AppException(new Error("INVALID_OPERATION", "Cannot add members to a direct message"));
            }

            var actor = await _userRepository.GetAsync(actorId, u => u)
                ?? throw new NotFoundException(new Error("USER_NOT_FOUND", "User not found"));

            var existingParticipantIds = conversation.Participants.Select(p => p.UserId).ToHashSet();
            var newIds = participantIds.Where(id => !existingParticipantIds.Contains(id)).Distinct().ToList();

            if (newIds.Count == 0)
            {
                return Result.Create(ResponseStatusCode.Success);
            }

            var newUsers = await _userRepository.GetAllAsync(
                u => newIds.Contains(u.Id),
                u => new { u.Id, u.FullName }
            );

            foreach (var newId in newIds)
            {
                var participant = new ConversationParticipant
                {
                    ConversationId = conversationId,
                    UserId = newId,
                    Role = ConversationRole.Member,
                    CreatedAt = DateTime.UtcNow,
                };
                conversation.Participants.Add(participant);
            }

            UpdateSearchTextInternal(conversation);
            await _conversationRepository.UpdateAsync(conversation);

            var addedUserNames = newUsers.Select(u => u.FullName ?? "Thành viên mới").ToList();
            var addedUserNamesStr = string.Join(", ", addedUserNames);

            await _messageService.SendMessageAsync(
                actorId,
                new CreateMessageRequest
                {
                    ConversationId = conversationId,
                    Content = $"{actor.FullName} đã thêm {addedUserNamesStr} vào nhóm",
                    Type = MessageType.AddParticipant,
                    Metadata = new Dictionary<string, object>
                    {
                        { "actorId", actorId.ToString() },
                        { "actorName", actor.FullName! },
                        { "addedUserIds", newIds.Select(id => id.ToString()).ToList() },
                        { "addedUserNames", addedUserNames },
                    },
                }
            );

            return Result.Create(ResponseStatusCode.Success);
        }
    }
}

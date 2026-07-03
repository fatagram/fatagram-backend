using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.Configuration.Annotations;
using Fatagram.Application.Dtos.Filter;
using Fatagram.Application.Dtos.Message;
using Fatagram.Application.Exceptions.MiddleLevelExceptions;
using Fatagram.Application.Services.MessageServices;
using Fatagram.Application.Services.SocketServices;
using Fatagram.Application.Utils;
using Fatagram.Domain.Enums;
using Fatagram.Domain.Models;
using Fatagram.Application.Abstractions.Cache;
using Fatagram.Application.Common.Projections;
using Fatagram.Application.Abstractions.Repositories;
using Fatagram.Shared.Common;
using Fatagram.Shared.Enums;
using Fatagram.Shared.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Fatagram.Application.Services.MessageServices
{
    public class MessageService(
        IMessageRepository messageRepository,
        ISocketSender<ResponseMessageDto> messageSender,
        IConversationRepository conversationRepository,
        IConversationParticipantRepository cpRepo,
        IUserRepository userRepository,
        ICacheService cacheService,
        IMapper mapper,
        ILogger<MessageService> logger
    ) : IMessageService
    {
        private readonly IMessageRepository _messageRepository = messageRepository;
        private readonly ISocketSender<ResponseMessageDto> _messageSender = messageSender;
        private readonly IConversationRepository _conversationRepository = conversationRepository;
        private readonly IConversationParticipantRepository _cpRepo = cpRepo;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly ICacheService _cacheService = cacheService;
        private readonly ILogger<MessageService> _logger = logger;
        private readonly IMapper _mapper = mapper;

        public async Task<Result<CursorResult<ResponseMessageDto, int>>> GetMessagesAsync(
            Guid conversationId,
            CursorFilter<int> filter
        )
        {
            if (filter.Cursor <= 0)
                filter.Cursor = int.MaxValue;

            var messages = await _messageRepository.GetMessages(
                conversationId,
                filter.Cursor,
                filter.SortDesc ?? true,
                filter.Limit
            );

            return Result<CursorResult<ResponseMessageDto, int>>.Create(
                ResponseStatusCode.Success,
                new CursorResult<ResponseMessageDto, int>
                {
                    Items = _mapper.Map<List<ResponseMessageDto>>(messages),
                    NextCursor = messages.Count > 0 ? messages.Last().SequenceNumber : null,
                    HasNext = messages.Count == filter.Limit,
                }
            );
        }

        public async Task<Result<List<ResponseMessageDto>>> GetDeltaMessagesAsync(
            Guid conversationId,
            int sinceSequenceNumber
        )
        {
            var messages = await _messageRepository.GetDeltaMessagesAsync(
                conversationId,
                sinceSequenceNumber
            );

            return Result<List<ResponseMessageDto>>.Create(
                ResponseStatusCode.Success,
                _mapper.Map<List<ResponseMessageDto>>(messages)
            );
        }

        public async Task<Result<ResponseMessageDto>> SendMessageAsync(
            Guid? senderId,
            CreateMessageRequest request
        )
        {
            // Get conversation
            ConversationProjection? conversation;
            if (request.ConversationId == null || request.ConversationId == Guid.Empty)
            {
                return await SendFirstMessageAsync(senderId ?? Guid.Empty, request);
            }
            else
            {
                conversation = await _conversationRepository.GetConversationById(
                    senderId ?? Guid.Empty,
                    request.ConversationId.Value,
                    c => new ConversationProjection
                    {
                        Id = c.Id,
                        IsGroup = c.IsGroup,
                        LastMessageNumber = c.LastMessageNumber,
                        ParticipantIds = c.Participants.Select(p => p.UserId).ToList(),
                        Participants = c
                            .Participants.Select(p => new ConversationParticipant
                            {
                                UserId = p.UserId,
                                Nickname = p.Nickname,
                                User =
                                    p.UserId == senderId
                                        ? new User
                                        {
                                            Id = p.User!.Id,
                                            FullName = p.User!.FullName,
                                            Avatar = p.User!.Avatar,
                                        }
                                        : null,
                                LastSeenNumber = p.LastSeenNumber,
                            })
                            .ToList(),
                    }
                );
            }

            if (conversation == null)
            {
                throw new NotFoundException(
                    new Error("CONVERSATION_NOT_FOUND", "Conversation not found")
                );
            }

            if (
                !conversation.ParticipantIds.Contains(senderId ?? Guid.Empty)
                && !IsSystemMessage(request.Type)
            )
            {
                throw new ForbiddenException(
                    new Error("FORBIDDEN", "You don't have permission to access this resource.")
                );
            }

            if (request.Type != MessageType.Text && (request.Media == null || !request.Media.Any()))
            {
                foreach (var media in request.Media ?? [])
                {
                    media.Id = Guid.NewGuid();
                }
            }

            var message = await _messageRepository.AddAsync(
                new Message
                {
                    ConversationId = conversation.Id,
                    SenderId = senderId,
                    Content = request.Content,
                    Type = request.Type,
                    Metadata = request.Metadata,
                    Sender = conversation
                        .Participants.FirstOrDefault(p => p.UserId == senderId)
                        ?.User!,
                    Media = _mapper.Map<List<MessageMedia>>(request.Media),
                }
            );

            await _cpRepo.MarkAsSeenAsync(
                conversation.Id,
                senderId ?? Guid.Empty,
                message.SequenceNumber,
                DateTime.UtcNow
            );

            if (message is null)
            {
                throw new Exception("Failed to send message");
            }

            var sender = conversation.Participants.FirstOrDefault(p => p.UserId == senderId);
            IEnumerable<MessageMediaDto>? mediaDtos = null;

            if (message.Media != null && message.Media.Any(m => m.Type == MediaType.Image))
            {
                mediaDtos = _mapper.Map<List<MessageMediaDto>>(message.Media.Take(3));
            }
            else
            {
                mediaDtos = _mapper.Map<List<MessageMediaDto>>(message.Media);
            }

            var sendTasks = new List<Task>();
            var updateTasks = new List<Task>();

            foreach (var p in conversation.ParticipantIds)
            {
                if (p == senderId)
                    continue;
                bool shouldIncreaseUnreadCount = !(
                    await _conversationRepository.GetUnreadConversationsAsync(p)
                ).Any(c => c.ConversationId == conversation.Id && c.UnreadCount > 0);

                var resp = new ResponseMessageDto
                {
                    Id = message.Id,
                    ConversationId = conversation.Id,
                    SenderId = senderId,
                    SenderFullName = sender?.User?.FullName,
                    SenderAvatarUrl = sender?.User?.Avatar,
                    SenderNickname = sender?.Nickname,
                    SequenceNumber = message.SequenceNumber,
                    Content = request.Content,
                    Type = request.Type,
                    IsGroup = conversation.IsGroup,
                    Metadata = request.Metadata,
                    CreatedAt = message.CreatedAt,
                    ShouldIncreaseUnreadCount = shouldIncreaseUnreadCount,
                    Media = mediaDtos,
                };

                sendTasks.Add(
                    _messageSender.SendAsync(
                        p,
                        new SocketMessage<ResponseMessageDto>
                        {
                            Event = "NewMessage",
                            Payload = resp,
                        }
                    )
                );

                var userConvKeys = $"user:{p}:conversations_rank";

                updateTasks.Add(
                    _cacheService.SortedSetAddAsync(
                        userConvKeys,
                        conversation.Id.ToString(),
                        DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                    )
                );
            }

            var response = new ResponseMessageDto
            {
                Id = message.Id,
                ConversationId = conversation.Id,
                SenderId = senderId,
                SenderFullName = sender?.User?.FullName,
                SenderAvatarUrl = sender?.User?.Avatar,
                SenderNickname = sender?.Nickname,
                SequenceNumber = message.SequenceNumber,
                ClientTempId = request.ClientTempId,
                Content = request.Content,
                Type = request.Type,
                IsGroup = conversation.IsGroup,
                Metadata = request.Metadata,
                CreatedAt = message.CreatedAt,
                ShouldIncreaseUnreadCount = false,
                Media = mediaDtos,
            };

            sendTasks.Add(
                _messageSender.SendAsync(
                    senderId ?? Guid.Empty,
                    new SocketMessage<ResponseMessageDto>
                    {
                        Event = "NewMessage",
                        Payload = response,
                    }
                )
            );
            updateTasks.Add(
                _cacheService.SortedSetAddAsync(
                    $"user:{senderId}:conversations_rank",
                    conversation.Id.ToString(),
                    DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                )
            );

            await Task.WhenAll([.. sendTasks, .. updateTasks]);

            await _conversationRepository.NotifyNewMessage(
                conversation.Id,
                senderId ?? Guid.Empty,
                conversation.ParticipantIds
            );

            return Result<ResponseMessageDto>.Create(ResponseStatusCode.Success, response);
        }

        private async Task<Result<ResponseMessageDto>> SendFirstMessageAsync(
            Guid senderId,
            CreateMessageRequest request
        )
        {
            if (request.ReceiverId == null)
            {
                throw new ArgumentException(
                    "ReceiverId must be provided if ConversationId is not provided"
                );
            }

            var uniqueKey = ConversationUtils.GenerateUniqueConversationKey(
                senderId,
                request.ReceiverId.Value
            );

            var message = await _conversationRepository.ExecuteTransactionAction(async () =>
            {
                var participants = new List<ConversationParticipant>
                {
                    new() { UserId = senderId, LastSeenNumber = 1 },
                };

                if (request.ReceiverId != senderId)
                {
                    participants.Add(
                        new() { UserId = request.ReceiverId ?? Guid.Empty, LastSeenNumber = 0 }
                    );
                }

                var _res = await _conversationRepository.AddAsync(
                    new Conversation
                    {
                        IsGroup = false,
                        Name = string.Empty,
                        UniqueConversationKey = uniqueKey,
                        Participants = participants,
                    }
                );
                var conversation = _mapper.Map<ConversationProjection>(_res);
                var userSender = await _userRepository.GetByUniqueAsync(
                    u => u.Id == senderId,
                    u => u
                );
                if (userSender == null)
                {
                    throw new Exception("Sender user not found");
                }

                var message = await _messageRepository.AddAsync(
                    new Message
                    {
                        ConversationId = conversation.Id,
                        SenderId = senderId,
                        Content = request.Content,
                        Type = request.Type,
                        Metadata = request.Metadata,
                    }
                );

                await _conversationRepository.NotifyNewMessage(
                    conversation.Id,
                    senderId,
                    conversation.ParticipantIds
                );

                var resp = new ResponseMessageDto
                {
                    Id = message.Id,
                    ConversationId = message.ConversationId,
                    SenderId = senderId,
                    SenderFullName = userSender?.FullName,
                    SenderAvatarUrl = userSender?.Avatar,
                    SenderNickname = userSender?.Nickname,
                    SequenceNumber = message.SequenceNumber,
                    CorrelationId = request.CorrelationId,
                    ClientTempId = request.ClientTempId,
                    Content = request.Content,
                    Type = request.Type,
                    IsGroup = conversation.IsGroup,
                    Metadata = request.Metadata,
                    CreatedAt = message.CreatedAt,
                };

                await _messageSender.SendAllAsync(
                    conversation.ParticipantIds,
                    new SocketMessage<ResponseMessageDto> { Event = "NewMessage", Payload = resp }
                );

                foreach (var participantId in conversation.ParticipantIds)
                {
                    await _cacheService.SortedSetAddAsync(
                        $"user:{participantId}:conversations_rank",
                        conversation.Id.ToString(),
                        DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                    );
                }

                return resp;
            });

            return Result<ResponseMessageDto>.Create(ResponseStatusCode.Success, message);
        }

        private static bool IsSystemMessage(MessageType type)
        {
            return type == MessageType.System
                || type == MessageType.CreateGroup
                || type == MessageType.ChangeGroupAvatar
                || type == MessageType.RenameGroup
                || type == MessageType.ChangeBackgroundUrl
                || type == MessageType.ChangeTheme;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using AutoMapper;
using Fatagram.Application.Dtos.Filter;
using Fatagram.Application.Dtos.Message;
using Fatagram.Application.Services.MessageServices.Interfaces;
using Fatagram.Application.Services.SockerServices.Interfaces;
using Fatagram.Application.Utils;
using Fatagram.Domain.Models;
using Fatagram.Infrastructure.Cache;
using Fatagram.Infrastructure.Projections;
using Fatagram.Infrastructure.Repositories.ConversationRepository.Interfaces;
using Fatagram.Infrastructure.Repositories.MessageRepository.Interfaces;
using Fatagram.Infrastructure.Repositories.UserRepository.Interface;
using Fatagram.Shared.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Logging;

namespace Fatagram.Application.Services.MessageServices
{
    public class MessageService(
        IMessageRepository messageRepository,
        ISocketSender<ResponseMessageDto> messageSender,
        IConversationRepository conversationRepository,
        IUserRepository userRepository,
        IMapper mapper,
        ICacheService cacheService,
        ILogger<MessageService> logger
    ) : IMessageService
    {
        private readonly IMessageRepository _messageRepository = messageRepository;
        private readonly ISocketSender<ResponseMessageDto> _messageSender = messageSender;
        private readonly IConversationRepository _conversationRepository = conversationRepository;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly ILogger<MessageService> _logger = logger;
        private readonly ICacheService _cacheService = cacheService;
        private readonly IMapper _mapper = mapper;

        public async Task<Result<CursorResult<ResponseMessageDto, DateTime>>> GetMessagesAsync(
            Guid conversationId,
            Guid userId,
            CursorFilter<DateTime> filter
        )
        {
            var messages = await _messageRepository.GetAllAsync(
                m => new ResponseMessageDto
                {
                    Id = m.Id,
                    ConversationId = m.ConversationId,
                    SenderId = m.SenderId,
                    SenderFullName = m.Sender.FullName!,
                    SenderAvatarUrl = m.Sender.Avatar,
                    SenderNickname = m
                        .Sender.ConversationParticipants.Where(cp =>
                            cp.ConversationId == m.ConversationId && cp.UserId == m.SenderId
                        )
                        .Select(cp => cp.Nickname)
                        .FirstOrDefault(),
                    Type = m.Type,
                    Metadata = m.Metadata,
                    Content = m.Content,
                    CreatedAt = m.CreatedAt,
                    IsGroup = m.Conversation.IsGroup,
                },
                m => m.ConversationId == conversationId,
                m => m.CreatedAt,
                filter.SortDesc ?? true,
                filter.Limit,
                filter.Cursor
            );
            return Result<CursorResult<ResponseMessageDto, DateTime>>.Create(
                ResponseStatusCode.Success,
                new CursorResult<ResponseMessageDto, DateTime>
                {
                    Items = messages,
                    NextCursor = messages.Count > 0 ? messages.Last().CreatedAt : null,
                    HasNext = messages.Count == filter.Limit,
                }
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
                Console.WriteLine(
                    request.ReceiverId == null
                        ? "No ConversationId provided, but ReceiverId is also null. This will cause an error."
                        : $"No ConversationId provided, generating unique conversation key for sender {senderId} and receiver {request.ReceiverId}"
                );
                if (request.ReceiverId == null)
                {
                    throw new ArgumentException(
                        "ReceiverId must be provided if ConversationId is not provided"
                    );
                }
                var uniqueKey = ConversationUtils.GenerateUniqueConversationKey(
                    senderId ?? Guid.Empty,
                    request.ReceiverId.Value
                );
                try
                {
                    var res = await _conversationRepository.AddAsync(
                        new Conversation
                        {
                            IsGroup = false,
                            Name = string.Empty,
                            UniqueConversationKey = uniqueKey,
                            Participants = new List<ConversationParticipant>
                            {
                                new ConversationParticipant
                                {
                                    UserId = request.ReceiverId ?? Guid.Empty,
                                    LastSeenNumber = 0,
                                },
                                new ConversationParticipant
                                {
                                    UserId = senderId ?? Guid.Empty,
                                    LastSeenNumber = 1,
                                },
                            },
                        }
                    );
                    conversation = _mapper.Map<ConversationProjection>(res);
                    var userSender = await _userRepository.GetByUniqueAsync(
                        u => u.Id == senderId,
                        u => u
                    );
                    if (userSender == null)
                    {
                        throw new Exception("Sender user not found");
                    }
                    conversation.Participants = new List<ConversationParticipant>
                    {
                        new ConversationParticipant { UserId = request.ReceiverId ?? Guid.Empty },
                        new ConversationParticipant
                        {
                            UserId = senderId ?? Guid.Empty,
                            User = userSender,
                        },
                    };
                }
                catch (DbUpdateException)
                {
                    var existingConv = await _conversationRepository.GetByUniqueAsync(
                        c => c.UniqueConversationKey == uniqueKey,
                        c => new ConversationProjection
                        {
                            Id = c.Id,
                            IsGroup = c.IsGroup,
                            LastMessageNumber = c.LastMessageNumber,
                            ParticipantIds = c.Participants.Select(p => p.UserId).ToList(),
                            Participants = c
                                .Participants.Select(p => new ConversationParticipant
                                {
                                    Id = p.UserId,
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
                    conversation = existingConv;
                }
            }
            else
            {
                Console.WriteLine(
                    $"Fetching conversation with ID {request.ConversationId} for sender {senderId}"
                );
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
                throw new Exception("Conversation not found");
            }

            var cachedMsgNum = await _cacheService.HashGetAsync(
                $"conv:{conversation.Id}:meta",
                "max_seq"
            );

            if (string.IsNullOrEmpty(cachedMsgNum))
            {
                await _cacheService.HashSetAsync(
                    $"conv:{conversation.Id}:meta",
                    "max_seq",
                    conversation.LastMessageNumber.ToString()
                );
            }

            var newMsgNum = await _cacheService.HashIncrementAsync(
                $"conv:{conversation.Id}:meta",
                "max_seq",
                1
            );
            int currentMaxSeq = (int)newMsgNum - 1;

            var message = await _messageRepository.AddAsync(
                new Message
                {
                    ConversationId = conversation.Id,
                    SenderId = senderId,
                    Content = request.Content,
                    Type = request.Type,
                    Metadata = request.Metadata,
                    SequenceNumber = (int)newMsgNum,
                }
            );

            await _cacheService.SetAsync(
                $"msg:seq:{message.Id}",
                message.SequenceNumber,
                TimeSpan.FromSeconds(7)
            );

            if (message == null)
            {
                _logger.LogError(
                    "Failed to save message for conversation {ConversationId} from sender {SenderId}",
                    conversation.Id,
                    senderId
                );
                throw new Exception("Failed to send message");
            }
            if (conversation.ParticipantIds == null || conversation.ParticipantIds.Count == 0)
            {
                _logger.LogError(
                    "No participants found for conversation {ConversationId}",
                    conversation.Id
                );
                throw new Exception("No participants found for conversation");
            }
            var sender = conversation.Participants.FirstOrDefault(p => p.UserId == senderId);

            await _cacheService.HashSetAsync(
                $"user:{senderId}:last_read",
                conversation.Id.ToString(),
                newMsgNum.ToString()
            );

            var sendTasks = conversation.Participants.Select(async p =>
            {
                bool isStartingFromRead = false;

                var lrStr = await _cacheService.HashGetAsync(
                    $"user:{p.UserId}:last_read",
                    conversation.Id.ToString()
                );

                int lastRead;
                if (lrStr != null && int.TryParse(lrStr, out var lr))
                {
                    lastRead = lr;
                }
                else
                {
                    lastRead = p.LastSeenNumber;
                    await _cacheService.HashSetAsync(
                        $"user:{p.UserId}:last_read",
                        conversation.Id.ToString(),
                        lastRead.ToString()
                    );
                }

                isStartingFromRead = currentMaxSeq <= lastRead;

                if (p.UserId == senderId)
                {
                    isStartingFromRead = false;
                }

                _logger.LogInformation(
                    "[BadgeTrace-NewMessage] ConversationId={ConversationId}, RecipientUserId={RecipientUserId}, SenderId={SenderId}, MessageId={MessageId}, MessageSeq={MessageSeq}, CurrentMaxSeqBeforeSend={CurrentMaxSeqBeforeSend}, RecipientLastRead={RecipientLastRead}, IsConversationStartingFromRead={IsConversationStartingFromRead}, CorrelationId={CorrelationId}, ClientTempId={ClientTempId}",
                    conversation.Id,
                    p.UserId,
                    senderId,
                    message.Id,
                    message.SequenceNumber,
                    currentMaxSeq,
                    lastRead,
                    isStartingFromRead,
                    request.CorrelationId,
                    request.ClientTempId
                );

                var dto = new ResponseMessageDto
                {
                    Id = message.Id,
                    ConversationId = conversation.Id,
                    SenderId = senderId,
                    SenderFullName = sender?.User?.FullName,
                    SenderAvatarUrl = sender?.User?.Avatar,
                    SenderNickname = sender?.Nickname,
                    CorrelationId = request.CorrelationId,
                    ClientTempId = request.ClientTempId,
                    Content = request.Content,
                    Type = request.Type,
                    IsGroup = conversation.IsGroup,
                    Metadata = request.Metadata,
                    CreatedAt = message.CreatedAt,
                    IsConversationStartingFromRead = isStartingFromRead,
                };

                await _messageSender.SendAsync(
                    p.UserId,
                    new SocketMessage<ResponseMessageDto> { Event = "NewMessage", Payload = dto }
                );
            });

            await Task.WhenAll(sendTasks);

            return Result<ResponseMessageDto>.Create(
                ResponseStatusCode.Success,
                new ResponseMessageDto
                {
                    Id = message.Id,
                    ConversationId = conversation.Id,
                    SenderId = senderId,
                    SenderFullName = sender?.User?.FullName,
                    SenderAvatarUrl = sender?.User?.Avatar,
                    SenderNickname = sender?.Nickname,
                    CorrelationId = request.CorrelationId,
                    ClientTempId = request.ClientTempId,
                    Content = request.Content,
                    Type = request.Type,
                    IsGroup = conversation.IsGroup,
                    Metadata = request.Metadata,
                    CreatedAt = message.CreatedAt,
                }
            );
        }
    }
}

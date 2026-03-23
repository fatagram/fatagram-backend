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
using Fatagram.Infrastructure.Projections;
using Fatagram.Infrastructure.Repositories.ConversationRepository.Interfaces;
using Fatagram.Infrastructure.Repositories.MessageRepository.Interfaces;
using Fatagram.Shared.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Fatagram.Application.Services.MessageServices
{
    public class MessageService(
        IMessageRepository messageRepository,
        ISocketSender<ResponseMessageDto> messageSender,
        IConversationRepository conversationRepository,
        IMapper mapper,
        ILogger<MessageService> logger
    ) : IMessageService
    {
        private readonly IMessageRepository _messageRepository = messageRepository;
        private readonly ISocketSender<ResponseMessageDto> _messageSender = messageSender;
        private readonly IConversationRepository _conversationRepository = conversationRepository;
        private readonly ILogger<MessageService> _logger = logger;
        private readonly IMapper _mapper = mapper;

        public async Task<Result<CursorResult<ResponseMessageDto, DateTime>>> GetMessagesAsync(
            Guid conversationId,
            Guid userId,
            CursorFilter<DateTime> filter
        )
        {
            Console.WriteLine(
                $"Getting messages for conversation {conversationId} with cursor {filter.Cursor} and limit {filter.Limit} and sortDesc {filter.SortDesc}"
            );
            var messages = await _messageRepository.GetAllAsync(
                m => new ResponseMessageDto
                {
                    Id = m.Id,
                    ConversationId = m.ConversationId,
                    SenderId = m.SenderId,
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
            Guid senderId,
            SendMessageDto request
        )
        {
            _logger.LogInformation(
                "User {SenderId} is sending a message to conversation {ConversationId} or {CorrelationId}",
                senderId,
                request.ConversationId,
                request.CorrelationId
            );
            ConversationProjection? conversation;
            if (request.ConversationId == null || request.ConversationId == Guid.Empty)
            {
                _logger.LogInformation(
                    "No conversation ID provided, checking for existing conversation between sender {SenderId} and receiver {ReceiverId}",
                    senderId,
                    request.ReceiverId
                );
                if (request.ReceiverId == null)
                {
                    throw new ArgumentException(
                        "ReceiverId must be provided if ConversationId is not provided"
                    );
                }
                var res = await _conversationRepository.AddAsync(
                    new Conversation
                    {
                        IsGroup = false,
                        Name = string.Empty,
                        Participants = new List<ConversationParticipant>
                        {
                            new ConversationParticipant
                            {
                                UserId = request.ReceiverId ?? Guid.Empty,
                            },
                            new ConversationParticipant { UserId = senderId },
                        },
                    }
                );
                conversation = _mapper.Map<ConversationProjection>(res);
            }
            else
            {
                conversation = await _conversationRepository.GetConversationById(
                    senderId,
                    request.ConversationId.Value
                );
            }

            if (conversation == null)
            {
                throw new Exception("Conversation not found");
            }

            Console.WriteLine($"Conversation ID: {conversation.Id}");

            var message = await _messageRepository.AddAsync(
                new Message
                {
                    ConversationId = conversation.Id,
                    SenderId = senderId,
                    Content = request.Content,
                }
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

            foreach (var participantId in conversation.ParticipantIds)
            {
                await _messageSender.SendAsync(
                    participantId,
                    new SocketMessage<ResponseMessageDto>
                    {
                        Event = "NewMessage",
                        Payload = new ResponseMessageDto
                        {
                            Id = message.Id,
                            ConversationId = conversation.Id,
                            CorrelationId = request.CorrelationId,
                            SenderId = senderId,
                            Content = request.Content,
                            CreatedAt = message.CreatedAt,
                            IsGroup = conversation.IsGroup,
                        },
                    }
                );
            }
            return Result<ResponseMessageDto>.Create(
                ResponseStatusCode.Success,
                new ResponseMessageDto
                {
                    Id = message.Id,
                    ConversationId = conversation.Id,
                    SenderId = senderId,
                    CorrelationId = request.CorrelationId,
                    Content = request.Content,
                    CreatedAt = message.CreatedAt,
                    IsGroup = conversation.IsGroup,
                }
            );
        }
    }
}

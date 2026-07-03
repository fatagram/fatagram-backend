using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Fatagram.Application.Dtos.Conversation;
using Fatagram.Application.Exceptions.MiddleLevelExceptions;
using Fatagram.Application.Services.ConversationServices;
using Fatagram.Application.Services.SocketServices;
using Fatagram.Application.Utils;
using Fatagram.Application.Abstractions.Cache;
using Fatagram.Application.Common.Projections;
using Fatagram.Application.Abstractions.Repositories;
using Fatagram.Shared.Common;
using Fatagram.Shared.Enums;
using Microsoft.Extensions.Logging;

namespace Fatagram.Application.Services.ConversationServices
{
    public class ConversationParticipantService(
        IConversationParticipantRepository conversationParticipantRepository,
        IConversationRepository conversationRepository,
        ISocketSender<SeenDto> sockerSender
    ) : IConversationParticipantService
    {
        private readonly IConversationParticipantRepository _cpRepo =
            conversationParticipantRepository;
        private readonly IConversationRepository _conversationRepository = conversationRepository;

        private readonly ISocketSender<SeenDto> _sockerSender = sockerSender;

        public async Task<Result> MarkAsSeenAsync(Guid conversationId, Guid userId, int messageSeq)
        {
            var seenAt = DateTime.UtcNow;

            var participant = await _conversationRepository.GetConversationById(
                userId,
                conversationId,
                c => new() { ParticipantIds = c.Participants.Select(p => p.UserId).ToList() }
            );

            bool shouldDecreaseUnreadCount =
                (await _conversationRepository.GetUnreadConversationsAsync(userId)).FirstOrDefault(
                    c => c.ConversationId == conversationId && c.UnreadCount > 0
                ) != null;

            var sendForUserTask = _sockerSender.SendAsync(
                userId,
                new SocketMessage<SeenDto>
                {
                    Event = "SeenMessage",
                    Payload = new SeenDto
                    {
                        ConversationId = conversationId,
                        UserId = userId,
                        MessageSeq = messageSeq,
                        SeenAt = seenAt,
                        shouldDecreaseUnreadCount = shouldDecreaseUnreadCount,
                    },
                }
            );

            var sendForOthersTasks = (participant?.ParticipantIds ?? Enumerable.Empty<Guid>())
                .Where(id => id != userId)
                .Select(id =>
                    _sockerSender.SendAsync(
                        id,
                        new SocketMessage<SeenDto>
                        {
                            Event = "SeenMessage",
                            Payload = new SeenDto
                            {
                                ConversationId = conversationId,
                                UserId = userId,
                                MessageSeq = messageSeq,
                                SeenAt = seenAt,
                                shouldDecreaseUnreadCount = false,
                            },
                        }
                    )
                );

            await _cpRepo.MarkAsSeenAsync(conversationId, userId, messageSeq, seenAt);

            var allTasks = new List<Task> { sendForUserTask };
            allTasks.AddRange(sendForOthersTasks);

            await Task.WhenAll(allTasks);

            return Result.Create(ResponseStatusCode.Success);
        }

        public async Task<Result<ParticipantsSeenProjection>> GetParticipantSeenAsync(
            Guid conversationId
        )
        {
            var participantsSeenInfo = await _cpRepo.GetConversationParticipantsSeenInfoAsync(
                conversationId
            );

            return Result<ParticipantsSeenProjection>.Create(
                ResponseStatusCode.Success,
                participantsSeenInfo
            );
        }
    }
}

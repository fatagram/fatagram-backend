using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Fatagram.Application.Dtos.Conversation;
using Fatagram.Application.Exceptions.MiddleLevelExceptions;
using Fatagram.Application.Services.ConversationServices.Interfaces;
using Fatagram.Application.Services.SockerServices.Interfaces;
using Fatagram.Application.Utils;
using Fatagram.Infrastructure.Cache;
using Fatagram.Infrastructure.Repositories.ConversationParticipantRepository.Interfaces;
using Fatagram.Shared.Common;
using Fatagram.Shared.Enums;

namespace Fatagram.Application.Services.ConversationServices
{
    public class ConversationParticipantService(
        IConversationParticipantRepository conversationParticipantRepository,
        ICacheService cacheService,
        ISocketSender<SeenDto> sockerSender
    ) : IConversationParticipantService
    {
        private readonly IConversationParticipantRepository _cpRepo =
            conversationParticipantRepository;
        private readonly ISocketSender<SeenDto> _sockerSender = sockerSender;
        private readonly ICacheService _cacheService = cacheService;

        public async Task<Result> MarkAsReadAsync(Guid conversationId, Guid userId, Guid messageId)
        {
            Console.WriteLine(
                $"MarkAsReadAsync called with conversationId={conversationId}, userId={userId}, messageId={messageId}"
            );
            var seenAt = DateTime.UtcNow;

            var cacheValue = JsonSerializer.Serialize(
                new ParticipantSeenInfo { MessageId = messageId, SeenAt = seenAt }
            );
            // Check user is participant of the conversation
            await _cacheService.HashSetAsync(
                $"conv:{conversationId}:seen",
                userId.ToString(),
                cacheValue
            );
            var participants = await GetAllParticipantsAsync(conversationId, userId);
            var seenDto = new SeenDto
            {
                ConversationId = conversationId,
                UserId = userId,
                MessageId = messageId,
                SeenAt = seenAt,
            };
            await _sockerSender.SendAllAsync(
                participants.Data?.Select(p => p.UserId).ToList() ?? [],
                new SocketMessage<SeenDto> { Event = "SeenMessage", Payload = seenDto }
            );
            return Result.Create(ResponseStatusCode.Success);
        }

        public async Task<Result<List<ParticipantDto>>> GetAllParticipantsAsync(
            Guid conversationId,
            Guid userId
        )
        {
            var cacheKey = $"conversation:{conversationId}:participants";
            var cachedParticipants = await _cacheService.GetAsync<List<ParticipantDto>>(cacheKey);

            if (cachedParticipants != null)
            {
                // Check user is participant of the conversation
                if (!cachedParticipants.Any(p => p.UserId == userId))
                {
                    throw new ForbiddenException(
                        new Error(
                            "USER_NOT_PARTICIPANT",
                            "User is not a participant of the conversation"
                        )
                    );
                }

                return Result<List<ParticipantDto>>.Create(
                    ResponseStatusCode.Success,
                    cachedParticipants
                );
            }

            // Check user is participant of the conversation
            if (
                await _cpRepo.CountAsync(cp =>
                    cp.ConversationId == conversationId && cp.UserId == userId
                ) == 0
            )
            {
                throw new ForbiddenException(
                    new Error(
                        "USER_NOT_PARTICIPANT",
                        "User is not a participant of the conversation"
                    )
                );
            }

            var participants = await _cpRepo.GetAllAsync(
                cp => cp.ConversationId == conversationId,
                cp => new ParticipantDto { UserId = cp.UserId, CreatedAt = cp.CreatedAt }
            );
            await _cacheService.SetAsync(cacheKey, participants, TimeSpan.FromDays(2));

            return Result<List<ParticipantDto>>.Create(ResponseStatusCode.Success, participants);
        }

        public async Task<Result<ParticipantsSeenDto>> GetParticipantSeenAsync(
            Guid conversationId,
            Guid userId
        )
        {
            var participantsSeenInfo = new Dictionary<Guid, ParticipantSeenInfo>();

            var participantsSeenOnDb = await _cpRepo.GetAllAsync(
                cp => cp.ConversationId == conversationId,
                cp => new
                {
                    cp.UserId,
                    cp.LastSeenMessageId,
                    cp.SeenAt,
                }
            );

            foreach (var p in participantsSeenOnDb)
            {
                if (p.LastSeenMessageId.HasValue)
                {
                    participantsSeenInfo[p.UserId] = new ParticipantSeenInfo
                    {
                        MessageId = p.LastSeenMessageId.Value,
                        SeenAt = p.SeenAt ?? DateTime.MinValue,
                    };
                }
            }

            var cacheSeenData = await _cacheService.HashGetAllAsync($"conv:{conversationId}:seen");

            foreach (var entry in cacheSeenData)
            {
                if (Guid.TryParse(entry.Key, out var pUserId))
                {
                    var cacheJson = entry.Value;
                    if (string.IsNullOrEmpty(cacheJson))
                        continue;

                    var cacheSeenInfo = JsonSerializer.Deserialize<ParticipantSeenInfo>(cacheJson);
                    if (cacheSeenInfo != null)
                    {
                        participantsSeenInfo[pUserId] = cacheSeenInfo;
                    }
                }
            }

            return Result<ParticipantsSeenDto>.Create(
                ResponseStatusCode.Success,
                new ParticipantsSeenDto
                {
                    conversationId = conversationId,
                    ParticipantsSeenInfo = participantsSeenInfo,
                }
            );
        }
    }
}

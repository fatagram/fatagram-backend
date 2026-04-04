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
using Fatagram.Infrastructure.Repositories.ConversationRepository.Interfaces;
using Fatagram.Infrastructure.Repositories.MessageRepository.Interfaces;
using Fatagram.Shared.Common;
using Fatagram.Shared.Enums;
using Microsoft.Extensions.Logging;

namespace Fatagram.Application.Services.ConversationServices
{
    public class ConversationParticipantService(
        IConversationParticipantRepository conversationParticipantRepository,
        IMessageRepository messageRepository,
        IConversationRepository conversationRepository,
        ICacheService cacheService,
        ISocketSender<SeenDto> sockerSender,
        ILogger<ConversationParticipantService> logger
    ) : IConversationParticipantService
    {
        private readonly IConversationParticipantRepository _cpRepo =
            conversationParticipantRepository;

        private readonly IMessageRepository _messageRepository = messageRepository;
        private readonly IConversationRepository _conversationRepository = conversationRepository;
        private readonly ISocketSender<SeenDto> _sockerSender = sockerSender;
        private readonly ICacheService _cacheService = cacheService;
        private readonly ILogger<ConversationParticipantService> _logger = logger;

        public async Task<Result> MarkAsReadAsync(Guid conversationId, Guid userId, Guid messageId)
        {
            var seenAt = DateTime.UtcNow;
            var seqNumber = await GetSequenceNumberAsync(messageId);

            var dedupeKey = $"seen:dedupe:{conversationId}:{userId}:{messageId}";
            var dedupeHit = await _cacheService.IncrementAsync(dedupeKey);
            if (dedupeHit > 1)
            {
                _logger.LogInformation(
                    "[BadgeTrace-SeenMessage-Deduped] ConversationId={ConversationId}, UserId={UserId}, MessageId={MessageId}, DedupeHit={DedupeHit}",
                    conversationId,
                    userId,
                    messageId,
                    dedupeHit
                );
                return Result.Create(ResponseStatusCode.Success);
            }

            var maxSeqStr = await _cacheService.HashGetAsync(
                $"conv:{conversationId}:meta",
                "max_seq"
            );

            int maxSeq;

            if (!int.TryParse(maxSeqStr, out maxSeq))
            {
                maxSeq = await _conversationRepository.GetByUniqueAsync(
                    c => c.Id == conversationId,
                    c => c.LastMessageNumber
                );
                await _cacheService.HashSetAsync(
                    $"conv:{conversationId}:meta",
                    "max_seq",
                    maxSeq.ToString()
                );
            }

            var cacheValue = JsonSerializer.Serialize(
                new ParticipantSeenInfo { MessageId = messageId, SeenAt = seenAt }
            );
            var oldUserLastReadStr = await _cacheService.HashGetAsync(
                $"user:{userId}:last_read",
                conversationId.ToString()
            );

            var participants = await GetAllParticipantsAsync(conversationId, userId);

            if (!int.TryParse(oldUserLastReadStr, out var oldUserLastRead))
            {
                oldUserLastRead = participants
                    .Data!.Where(p => p.UserId == userId)
                    .Select(p => p.LastMessageSequence ?? 0)
                    .FirstOrDefault();
            }

            var nextUserLastRead = Math.Max(oldUserLastRead, seqNumber);

            await Task.WhenAll(
                _cacheService.HashSetAsync(
                    $"conv:{conversationId}:seen",
                    userId.ToString(),
                    cacheValue
                ),
                _cacheService.HashSetAsync(
                    $"user:{userId}:last_read",
                    conversationId.ToString(),
                    nextUserLastRead.ToString()
                )
            );

            var sendOthersTasks = participants
                .Data!.Where(p => p.UserId != userId)
                .Select(async p =>
                {
                    // var lrStr = await _cacheService.HashGetAsync(
                    //     $"user:{p.UserId}:last_read",
                    //     conversationId.ToString()
                    // );
                    // int lastRead;

                    // if (lrStr != null && int.TryParse(lrStr, out var lr))
                    // {
                    //     lastRead = lr;
                    // }
                    // else
                    // {
                    //     lastRead = p.LastMessageSequence ?? 0;

                    //     await _cacheService.HashSetAsync(
                    //         $"user:{p.UserId}:last_read",
                    //         conversationId.ToString(),
                    //         lastRead.ToString()
                    //     );
                    // }

                    // bool isThisUserWasUnread = maxSeq > lastRead;

                    await _sockerSender.SendAsync(
                        p.UserId,
                        new SocketMessage<SeenDto>
                        {
                            Event = "SeenMessage",
                            Payload = new SeenDto
                            {
                                ConversationId = conversationId,
                                UserId = userId,
                                MessageId = messageId,
                                SeenAt = seenAt,
                            },
                        }
                    );
                });

            var wasUnreadBeforeRead = maxSeq > oldUserLastRead;
            var isFullyReadAfterRead = nextUserLastRead >= maxSeq;
            var hadUnreadFromOthersBeforeRead = false;

            if (wasUnreadBeforeRead)
            {
                hadUnreadFromOthersBeforeRead =
                    await _messageRepository.CountAsync(m =>
                        m.ConversationId == conversationId
                        && m.SenderId != userId
                        && m.SequenceNumber > oldUserLastRead
                    ) > 0;
            }

            var isPreviousUnread = hadUnreadFromOthersBeforeRead && isFullyReadAfterRead;

            _logger.LogInformation(
                "[BadgeTrace-SeenMessage] ConversationId={ConversationId}, UserId={UserId}, MessageId={MessageId}, SeqNumber={SeqNumber}, MaxSeq={MaxSeq}, OldUserLastRead={OldUserLastRead}, NextUserLastRead={NextUserLastRead}, WasUnreadBeforeRead={WasUnreadBeforeRead}, HadUnreadFromOthersBeforeRead={HadUnreadFromOthersBeforeRead}, IsFullyReadAfterRead={IsFullyReadAfterRead}, IsPreviousUnread={IsPreviousUnread}",
                conversationId,
                userId,
                messageId,
                seqNumber,
                maxSeq,
                oldUserLastRead,
                nextUserLastRead,
                wasUnreadBeforeRead,
                hadUnreadFromOthersBeforeRead,
                isFullyReadAfterRead,
                isPreviousUnread
            );
            var sendMeTask = _sockerSender.SendAsync(
                userId,
                new SocketMessage<SeenDto>
                {
                    Event = "SeenMessage",
                    Payload = new SeenDto
                    {
                        ConversationId = conversationId,
                        UserId = userId,
                        MessageId = messageId,
                        SeenAt = seenAt,
                        IsPreviousUnread = isPreviousUnread,
                    },
                }
            );

            await Task.WhenAll([.. sendOthersTasks, sendMeTask]);

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
                cp => new ParticipantDto
                {
                    UserId = cp.UserId,
                    CreatedAt = cp.CreatedAt,
                    LastMessageSequence = cp.LastSeenNumber,
                }
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

        private async Task<int> GetSequenceNumberAsync(Guid messageId)
        {
            var cacheKey = $"msg:seq:{messageId}";
            var cachedSeq = await _cacheService.GetAsync<int?>(cacheKey);
            _logger.LogDebug(
                "[BadgeTrace-SeqCache] MessageId={MessageId}, CachedSeq={CachedSeq}",
                messageId,
                cachedSeq
            );

            if (cachedSeq.HasValue)
            {
                return cachedSeq.Value;
            }

            var seqNumber = await _messageRepository.GetByUniqueAsync(
                m => m.Id == messageId,
                m => m.SequenceNumber
            );
            await _cacheService.SetAsync(cacheKey, seqNumber, TimeSpan.FromSeconds(7));
            _logger.LogDebug(
                "[BadgeTrace-SeqDb] MessageId={MessageId}, SeqNumber={SeqNumber}",
                messageId,
                seqNumber
            );

            return seqNumber;
        }
    }
}

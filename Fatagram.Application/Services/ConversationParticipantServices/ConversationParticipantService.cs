using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.Application.Dtos.Conversation;
using Fatagram.Application.Services.ConversationServices.Interfaces;
using Fatagram.Application.Services.SockerServices.Interfaces;
using Fatagram.Application.Utils;
using Fatagram.Infrastructure.Repositories.ConversationParticipantRepository.Interfaces;
using Fatagram.Shared.Enums;

namespace Fatagram.Application.Services.ConversationServices
{
    public class ConversationParticipantService(
        IConversationParticipantRepository conversationParticipantRepository,
        ISocketSender<SeenDto> sockerSender
    ) : IConversationParticipantService
    {
        private readonly IConversationParticipantRepository _cpRepo =
            conversationParticipantRepository;
        private readonly ISocketSender<SeenDto> _sockerSender = sockerSender;

        public async Task<Result> MarkAsReadAsync(Guid conversationId, Guid userId, Guid messageId)
        {
            await _cpRepo.UpdateAsync(
                cp => cp.ConversationId == conversationId && cp.UserId == userId,
                cp => cp.LastSeenMessageId = messageId
            );
            // _sockerSender.Send
            return Result.Create(ResponseStatusCode.Success);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.Application.Services.ConversationServices.Interfaces;
using Fatagram.Infrastructure.Repositories.ConversationParticipantRepository.Interfaces;

namespace Fatagram.Application.Services.ConversationServices
{
    public class ConversationParticipantService(
        IConversationParticipantRepository conversationParticipantRepository
    ) : IConversationParticipantService
    {
        private readonly IConversationParticipantRepository _cpRepo =
            conversationParticipantRepository;
    }
}

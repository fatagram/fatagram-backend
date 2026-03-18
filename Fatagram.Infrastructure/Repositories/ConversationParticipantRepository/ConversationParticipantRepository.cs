using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.Domain.Models;
using Fatagram.Infrastructure.Data;
using Fatagram.Infrastructure.Repositories.BaseRepository;
using Fatagram.Infrastructure.Repositories.ConversationParticipantRepository.Interfaces;
using Microsoft.Extensions.Logging;

namespace Fatagram.Infrastructure.Repositories.ConversationParticipantRepository
{
    public class ConversationParticipantRepository
        : BaseRepository<ConversationParticipant>,
            IConversationParticipantRepository
    {
        public ConversationParticipantRepository(
            AppDbContext dbContext,
            ILogger<BaseRepository<ConversationParticipant>>? logger = null
        )
            : base(dbContext, logger) { }
    }
}

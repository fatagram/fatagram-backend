using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fatagram.Application.Dtos.Conversation
{
    public class ParticipantSeenInfo
    {
        public Guid MessageId { get; set; }
        public DateTime SeenAt { get; set; }
    }

    public class ParticipantsSeenDto
    {
        public Guid conversationId { get; set; }
        public Dictionary<Guid, ParticipantSeenInfo> ParticipantsSeenInfo { get; set; } =
            new Dictionary<Guid, ParticipantSeenInfo>();
    }
}

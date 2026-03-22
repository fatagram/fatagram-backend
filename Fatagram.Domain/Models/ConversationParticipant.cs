using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fatagram.Domain.Models
{
    public class ConversationParticipant : BaseEntity
    {
        public Guid ConversationId { get; set; }
        public Guid UserId { get; set; }
        public Guid? LastSeenMessageId { get; set; }
        public virtual Conversation Conversation { get; set; } = null!;
        public virtual User User { get; set; } = null!;
        public virtual Message? LastSeenMessage { get; set; }
    }
}

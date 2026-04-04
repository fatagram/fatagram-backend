using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.Domain.Enums;

namespace Fatagram.Domain.Models
{
    public class ConversationParticipant : BaseEntity
    {
        public Guid ConversationId { get; set; }
        public Guid UserId { get; set; }
        public Guid? LastSeenMessageId { get; set; }
        public ConversationRole Role { get; set; }
        public string? Nickname { get; set; }
        public DateTime? SeenAt { get; set; }
        public int LastSeenNumber { get; set; }
        public virtual Conversation Conversation { get; set; } = null!;
        public virtual User? User { get; set; } = null!;
        public virtual Message? LastSeenMessage { get; set; }
    }
}

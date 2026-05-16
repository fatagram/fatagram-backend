using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fatagram.Domain.Models
{
    public class Conversation : BaseEntity
    {
        public string? Name { get; set; }
        public string? AvatarUrl { get; set; }
        public bool IsGroup { get; set; }
        public string? UniqueConversationKey { get; set; }
        public int LastMessageNumber { get; set; }
        public string SearchText { get; set; } = string.Empty;
        public virtual ICollection<ConversationParticipant> Participants { get; set; } = [];
        public virtual ICollection<Message> Messages { get; set; } = [];
    }
}

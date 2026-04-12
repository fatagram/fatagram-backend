using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Fatagram.Domain.Enums;

namespace Fatagram.Domain.Models
{
    public class Message : BaseEntity
    {
        public Guid ConversationId { get; set; }
        public Guid? SenderId { get; set; }
        public string Content { get; set; } = null!;
        public DateTime? ReadAt { get; set; }
        public MessageType Type { get; set; }
        public Dictionary<string, object>? Metadata { get; set; }
        public int SequenceNumber { get; set; }
        public virtual Conversation Conversation { get; set; } = null!;
        public virtual User Sender { get; set; } = null!;
        public virtual ICollection<MessageMedia>? Media { get; set; } = new List<MessageMedia>();
    }

    public class MessageMedia : BaseEntity
    {
        public Guid MessageId { get; set; }
        public string Url { get; set; } = null!;
        public MediaType Type { get; set; }
        public Dictionary<string, object>? Metadata { get; set; }
        public virtual Message Message { get; set; } = null!;
    }
}

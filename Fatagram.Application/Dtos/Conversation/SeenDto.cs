using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fatagram.Application.Dtos.Conversation
{
    public class SeenDto
    {
        public Guid ConversationId { get; set; }
        public Guid UserId { get; set; }
        public int MessageSeq { get; set; }
        public DateTime SeenAt { get; set; }
        public bool shouldDecreaseUnreadCount { get; set; }
    }
}

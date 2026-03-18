using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.Domain.Models;

namespace Fatagram.Infrastructure.Projections
{
    public class ConversationProjection : Conversation
    {
        public Message? LastMessage { get; set; }
        public int UnreadMessagesCount { get; set; }
        public DateTime LastActiveAt { get; set; }
    }
}

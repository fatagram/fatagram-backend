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
        public Guid MessageId { get; set; }
    }
}

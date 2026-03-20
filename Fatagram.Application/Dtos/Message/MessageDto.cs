using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fatagram.Application.Dtos.Message
{
    public class SendMessageDto
    {
        public Guid? ConversationId { get; set; }
        public Guid? ReceiverId { get; set; }
        public string Content { get; set; } = string.Empty;
    }

    public class ResponseMessageDto
    {
        public Guid Id { get; set; }
        public Guid ConversationId { get; set; }
        public Guid SenderId { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Fatagram.Application.Dtos.Message
{
    public class SendMessageDto
    {
        public Guid? ConversationId { get; set; }
        public Guid? ReceiverId { get; set; }
        public Guid? CorrelationId { get; set; }
        public Guid? ClientTempId { get; set; }
        public string Content { get; set; } = string.Empty;
    }

    public class ResponseMessageDto
    {
        public Guid Id { get; set; }
        public Guid? ConversationId { get; set; }
        public bool IsGroup { get; set; }
        public Guid SenderId { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Guid? CorrelationId { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Guid? ClientTempId { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}

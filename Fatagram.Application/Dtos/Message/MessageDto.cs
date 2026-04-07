using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Fatagram.Domain.Enums;

namespace Fatagram.Application.Dtos.Message
{
    public class SendMessageDto
    {
        public Guid? ConversationId { get; set; }
        public Guid? ReceiverId { get; set; }
        public string? CorrelationId { get; set; }
        public Guid? ClientTempId { get; set; }
        public string Content { get; set; } = string.Empty;
        public Dictionary<string, object>? Metadata { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public MessageType Type { get; set; } = MessageType.Text;
    }

    public class CreateMessageRequest
    {
        public Guid? ConversationId { get; set; }
        public Guid? ReceiverId { get; set; }
        public string? CorrelationId { get; set; }
        public Guid? ClientTempId { get; set; }
        public string Content { get; set; } = string.Empty;
        public MessageType Type { get; set; } = MessageType.Text;
        public Dictionary<string, object>? Metadata { get; set; }
    }

    public class ResponseMessageDto
    {
        public Guid Id { get; set; }
        public Guid? ConversationId { get; set; }
        public bool IsGroup { get; set; }
        public Guid? SenderId { get; set; }
        public string? SenderFullName { get; set; } = string.Empty;
        public string? SenderAvatarUrl { get; set; }
        public string? SenderNickname { get; set; }
        public int SequenceNumber { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? CorrelationId { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Guid? ClientTempId { get; set; }
        public string Content { get; set; } = string.Empty;

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public MessageType Type { get; set; } = MessageType.Text;
        public Dictionary<string, object>? Metadata { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool shouldIncreaseUnreadCount { get; set; }
    }
}

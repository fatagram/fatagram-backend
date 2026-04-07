using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Fatagram.Application.Dtos.Message;

namespace Fatagram.Application.Dtos.Conversation
{
    public class ConversationDto
    {
        public string Id { get; set; } = null!;
        public string? Name { get; set; }
        public string? AvatarUrl { get; set; }
        public bool IsGroup { get; set; }
        public ResponseMessageDto? LastMessage { get; set; }
        public int LastMessageNumber { get; set; }
        public int UnreadMessageCount { get; set; }
        public DateTime LastActiveAt { get; set; }
        public List<string>? TopParticipantNames { get; set; }
        public int? OtherLastSeenMessageSeq { get; set; }
        public int? MyLastSeenMessageSeq { get; set; }
        public int? ParticipantCount { get; set; }

        [JsonIgnore]
        public Guid? OtherUserId { get; set; }
    }
}

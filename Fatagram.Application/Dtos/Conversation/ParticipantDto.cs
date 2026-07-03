using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Fatagram.Application.Dtos.Conversation
{
    public record ParticipantDto
    {
        public Guid UserId { get; set; }

        public string Fullname { get; set; } = null!;

        public string? Nickname { get; set; }

        public string? AvatarUrl { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Guid? LastSeenMessageId { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? LastMessageSequence { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public DateTime? CreatedAt { get; set; }
    }
}

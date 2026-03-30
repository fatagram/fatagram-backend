using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.Domain.Models;

namespace Fatagram.Infrastructure.Projections
{
    public class LastMessageProjection : Message
    {
        public string? SenderFullName { get; set; } = string.Empty;
        public string? SenderNickname { get; set; }
    }

    public class ConversationProjection : Conversation
    {
        public LastMessageProjection? LastMessage { get; set; }
        public int UnreadMessagesCount { get; set; }
        public DateTime LastActiveAt { get; set; }
        public List<Guid>? ParticipantIds = new List<Guid>();
        public List<string>? TopParticipantNames = new List<string>();
        public Guid? OtherUserId { get; set; }
        public int? ParticipantCount { get; set; }
    }
}

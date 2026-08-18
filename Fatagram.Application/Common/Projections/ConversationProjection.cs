using System;
using System.Collections.Generic;
using Fatagram.Domain.Models;

namespace Fatagram.Application.Common.Projections
{
    public class LastMessageProjection : Message
    {
        public string? SenderFullName { get; set; } = string.Empty;
        public string? SenderNickname { get; set; }
    }

    public class ConversationProjection : Conversation
    {
        public LastMessageProjection? LastMessage { get; set; }
        public int UnreadMessageCount { get; set; }
        public DateTime LastActiveAt { get; set; }
        public List<Guid> ParticipantIds { get; set; } = [];
        public List<string> TopParticipantNames { get; set; } = [];
        public Guid? OtherUserId { get; set; }
        public int? ParticipantCount { get; set; }
        public DateTime? PinnedAt { get; set; }
        public bool IsPinned { get; set; }
    }

    public class ConversationSeenInfoProjection
    {
        public Guid ConversationId { get; set; }
        public int UnreadCount { get; set; }
    }

    public class ParticipantSeenInfoProjection
    {
        public DateTime SeenAt { get; set; }
        public int SequenceNumber { get; set; }
    }

    public class ParticipantsSeenProjection
    {
        public Guid ConversationId { get; set; }
        public Dictionary<Guid, ParticipantSeenInfoProjection> ParticipantsSeenInfo { get; set; } =
            new Dictionary<Guid, ParticipantSeenInfoProjection>();
    }

    public class MessageMediaAroundAnchorProjection
    {
        public List<MessageMedia> Left { get; set; } = [];
        public List<MessageMedia> Right { get; set; } = [];
    }
}

using System;
using System.Collections.Generic;

namespace Fatagram.Application.Dtos.Conversation
{
    public record AddParticipantsDto
    {
        public List<Guid> ParticipantIds { get; set; } = [];
    }
}

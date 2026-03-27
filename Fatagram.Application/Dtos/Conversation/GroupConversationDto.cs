using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fatagram.Application.Dtos.Conversation
{
    public class GroupConversationDto
    {
        public IEnumerable<Guid> ParticipantIds { get; set; } = new List<Guid>();
    }
}

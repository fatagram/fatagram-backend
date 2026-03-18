using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fatagram.Application.Dtos.Conversation
{
    public class ConversationDto
    {
        public string Id { get; set; } = null!;
        public string? Name { get; set; }
        public bool IsGroup { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Fatagram.Application.Dtos.Message;

namespace Fatagram.Application.Dtos.Conversation
{
    public class UpdateConversationDto
    {
        public string? Name { get; set; }
        public string? AvatarUrl { get; set; }
    }
}

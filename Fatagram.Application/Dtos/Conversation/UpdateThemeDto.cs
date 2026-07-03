using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fatagram.Application.Dtos.Conversation
{
    public record UpdateThemeDto
    {
        public string Theme { get; set; } = null!;
    }
}

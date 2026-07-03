using System;

namespace Fatagram.Application.Dtos.Conversation
{
    public record UpdateBackgroundDto
    {
        public string BackgroundUrl { get; set; } = null!;
    }
}

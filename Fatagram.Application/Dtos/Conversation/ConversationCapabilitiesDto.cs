using System;

namespace Fatagram.Application.Dtos.Conversation
{
    public record ConversationCapabilitiesDto
    {
        public bool CanSendMessage { get; set; } = true;
        public bool CanChangeAvatar { get; set; } = false;
        public bool CanChangeName { get; set; } = false;
        public bool CanChangeTheme { get; set; } = true;
        public bool CanChangeBackground { get; set; } = true;
        public bool CanKickMember { get; set; } = false;
        public bool CanAddMember { get; set; } = false;
        public bool CanPinMessage { get; set; } = false;
        public bool CanDeleteConversation { get; set; } = false;
    }
}

using Fatagram.API.Utils;
using Fatagram.Application.Abstractions.Storage.Enums;
using Fatagram.Application.Dtos.Conversation;
using Fatagram.Application.Dtos.Filter;
using Fatagram.Application.Services.ConversationServices;
using Fatagram.Application.Services.MediaServices;
using Fatagram.Application.Services.MessageServices;
using Microsoft.AspNetCore.Authorization;

namespace Fatagram.API.Controllers.V1
{
    [Authorize]
    public class ConversationController(
        IConversationService conversationService,
        IConversationParticipantService conversationParticipantService,
        IMessageService messageService,
        IMediaService mediaService,
        IImageService imageService,
        ILogger<ConversationController> logger
    ) : BaseApiController
    {
        private readonly ILogger<ConversationController> _logger = logger;
        private readonly IConversationService _conversationService = conversationService;
        private readonly IConversationParticipantService _conversationParticipantService =
            conversationParticipantService;
        private readonly IMessageService _messageService = messageService;
        private readonly IMediaService _mediaService = mediaService;
        private readonly IImageService _imageService = imageService;

        [HttpGet]
        public async Task<IActionResult> GetConversations(
            [FromQuery] CursorFilter<DateTime>? filter = null
        )
        {
            var res = await _conversationService.GetAllAsync(GetCurrentUserId(), filter);
            return res.ToActionResult();
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchConversations([FromQuery] CursorFilter<Guid> filter)
        {
            var res = await _conversationService.SearchAsync(GetCurrentUserId(), filter);
            return res.ToActionResult();
        }

        [HttpGet("delta")]
        public async Task<IActionResult> GetDeltaConversations([FromQuery] DateTime since)
        {
            var res = await _conversationService.GetDeltaAsync(GetCurrentUserId(), since);
            return res.ToActionResult();
        }

        [HttpGet("{conversationId}")]
        [ResourceAuth(ResourceType = "MemberConversation", RouteKey = "conversationId")]
        public async Task<IActionResult> GetConversationById(Guid conversationId)
        {
            var res = await _conversationService.GetByIdAsync(GetCurrentUserId(), conversationId);
            return res.ToActionResult();
        }

        [HttpGet("with/{targetUserId}")]
        public async Task<IActionResult> GetConversationWithUser(Guid targetUserId)
        {
            var res = await _conversationService.GetWithAsync(GetCurrentUserId(), targetUserId);
            return res.ToActionResult();
        }

        [HttpPost]
        public async Task<IActionResult> CreateGroupConversation(
            [FromBody] GroupConversationDto request
        )
        {
            var res = await _conversationService.CreateGroupAsync(
                GetCurrentUserId(),
                [.. request.ParticipantIds],
                request.Name
            );
            return res.ToActionResult();
        }

        [HttpGet("{conversationId}/messages")]
        [ResourceAuth(ResourceType = "MemberConversation", RouteKey = "conversationId")]
        public async Task<IActionResult> GetMessages(
            Guid conversationId,
            [FromQuery] CursorFilter<int> filter
        )
        {
            var res = await _messageService.GetMessagesAsync(conversationId, filter);
            return res.ToActionResult();
        }

        [HttpGet("{conversationId}/messages/delta")]
        [ResourceAuth(ResourceType = "MemberConversation", RouteKey = "conversationId")]
        public async Task<IActionResult> GetDeltaMessages(
            Guid conversationId,
            [FromQuery] int sinceSequenceNumber = 0
        )
        {
            var res = await _messageService.GetDeltaMessagesAsync(
                conversationId,
                sinceSequenceNumber
            );
            return res.ToActionResult();
        }

        [HttpGet("unread-count")]
        public async Task<IActionResult> GetUnreadNumberMessage()
        {
            var res = await _conversationService.GetUnreadCountAsync(GetCurrentUserId());
            return res.ToActionResult();
        }

        [HttpGet("{conversationId}/participants/seen")]
        [ResourceAuth(ResourceType = "MemberConversation", RouteKey = "conversationId")]
        public async Task<IActionResult> GetParticipantsSeen(Guid conversationId)
        {
            var res = await _conversationParticipantService.GetParticipantSeenAsync(conversationId);
            return res.ToActionResult();
        }

        [HttpGet("{conversationId}/participants")]
        [ResourceAuth(ResourceType = "MemberConversation", RouteKey = "conversationId")]
        public async Task<IActionResult> GetParticipants(
            Guid conversationId,
            [FromQuery] CursorFilter<DateTime> filter
        )
        {
            var res = await _conversationService.GetParticipantsAsync(
                GetCurrentUserId(),
                conversationId,
                filter
            );
            return res.ToActionResult();
        }

        [HttpPost("{conversationId}/messages/markSeen/{messageSeq}")]
        [ResourceAuth(ResourceType = "MemberConversation", RouteKey = "conversationId")]
        public async Task<IActionResult> MarkMessagesAsSeen(Guid conversationId, int messageSeq)
        {
            var res = await _conversationParticipantService.MarkAsSeenAsync(
                conversationId,
                GetCurrentUserId(),
                messageSeq
            );
            return res.ToActionResult();
        }

        [HttpGet("{conversationId}/media/around/{mediaId}")]
        [ResourceAuth(ResourceType = "MemberConversation", RouteKey = "conversationId")]
        public async Task<IActionResult> GetMediaAround(
            Guid conversationId,
            Guid mediaId,
            [FromQuery] int limit = 3,
            [FromQuery] bool before = true
        )
        {
            var res = await _mediaService.GetMediaAroundAsync(
                conversationId,
                mediaId,
                before,
                limit
            );
            return res.ToActionResult();
        }

        [HttpGet("{conversationId}/media/around-anchor/{mediaId}")]
        [ResourceAuth(ResourceType = "MemberConversation", RouteKey = "conversationId")]
        public async Task<IActionResult> GetMediaAroundAnchor(
            Guid conversationId,
            Guid mediaId,
            [FromQuery] int count = 5
        )
        {
            var res = await _mediaService.GetMediaAroundAnchorAsync(conversationId, mediaId, count);
            return res.ToActionResult();
        }

        [HttpPatch("{conversationId}/avatar")]
        [ResourceAuth(ResourceType = "MemberConversation", RouteKey = "conversationId")]
        public async Task<IActionResult> UpdateConversationAvatar(
            Guid conversationId,
            [FromForm] IFormFile file
        )
        {
            var res = await _imageService.SaveImageAsync(
                new ImageRequest()
                {
                    ImageStream = file.OpenReadStream(),
                    Format = ImageFormat.Jpeg,
                    Quality = ImageQuality.Medium,
                    SizePreset = ImageSizePreset.Medium,
                },
                "conversation-avatars"
            );

            var _res = await _conversationService.UpdateAvatarAsync(
                conversationId,
                res.Data!,
                GetCurrentUserId()
            );

            return _res.ToActionResult();
        }

        [HttpPatch("{conversationId}/name")]
        [ResourceAuth(ResourceType = "MemberConversation", RouteKey = "conversationId")]
        public async Task<IActionResult> UpdateConversationName(
            Guid conversationId,
            [FromBody] UpdateConversationDto request
        )
        {
            if (string.IsNullOrEmpty(request.Name))
            {
                return BadRequest("Name is required");
            }

            var res = await _conversationService.UpdateNameAsync(
                conversationId,
                request.Name,
                GetCurrentUserId()
            );

            return res.ToActionResult();
        }

        [HttpPatch("{conversationId}/background")]
        [ResourceAuth(ResourceType = "MemberConversation", RouteKey = "conversationId")]
        public async Task<IActionResult> UpdateConversationBackground(
            Guid conversationId,
            [FromBody] UpdateBackgroundDto request
        )
        {
            var res = await _conversationService.UpdateBackgroundUrlAsync(
                conversationId,
                request.BackgroundUrl,
                GetCurrentUserId()
            );

            return res.ToActionResult();
        }

        [HttpPatch("{conversationId}/theme")]
        [ResourceAuth(ResourceType = "MemberConversation", RouteKey = "conversationId")]
        public async Task<IActionResult> UpdateConversationTheme(
            Guid conversationId,
            [FromBody] UpdateThemeDto request
        )
        {
            if (string.IsNullOrEmpty(request.Theme))
            {
                return BadRequest("Theme is required");
            }

            var res = await _conversationService.UpdateThemeAsync(
                conversationId,
                request.Theme,
                GetCurrentUserId()
            );

            return res.ToActionResult();
        }
    }
}

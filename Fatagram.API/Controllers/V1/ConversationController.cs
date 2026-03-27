using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.API.Utils;
using Fatagram.Application.Dtos.Conversation;
using Fatagram.Application.Dtos.Filter;
using Fatagram.Application.Services.ConversationServices.Interfaces;
using Fatagram.Application.Services.MessageServices.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Fatagram.API.Controllers.V1
{
    [Authorize]
    public class ConversationController(
        IConversationService conversationService,
        IMessageService messageService,
        ILogger<ConversationController> logger
    ) : BaseApiController
    {
        private readonly ILogger<ConversationController> _logger = logger;
        private readonly IConversationService _conversationService = conversationService;
        private readonly IMessageService _messageService = messageService;

        [HttpGet]
        public async Task<IActionResult> GetConversations(
            [FromQuery] CursorFilter<DateTime>? filter = null
        )
        {
            var res = await _conversationService.GetAllAsync(GetCurrentUserId(), filter);
            return res.ToActionResult();
        }

        [HttpGet("{conversationId}")]
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
                request.ParticipantIds.ToList()
            );
            return res.ToActionResult();
        }

        [HttpGet("{conversationId}/messages")]
        public async Task<IActionResult> GetMessages(
            Guid conversationId,
            [FromQuery] CursorFilter<DateTime> filter
        )
        {
            var res = await _messageService.GetMessagesAsync(
                conversationId,
                GetCurrentUserId(),
                filter
            );
            return res.ToActionResult();
        }
    }
}

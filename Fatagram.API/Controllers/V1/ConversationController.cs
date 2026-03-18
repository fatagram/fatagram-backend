using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.API.Utils;
using Fatagram.Application.Dtos.Filter;
using Fatagram.Application.Services.ConversationServices.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Fatagram.API.Controllers.V1
{
    [Authorize]
    public class ConversationController(
        IConversationService conversationService,
        ILogger<ConversationController> logger
    ) : BaseApiController
    {
        private readonly ILogger<ConversationController> _logger = logger;
        private readonly IConversationService _conversationService = conversationService;

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
            var res = await _conversationService.GetAsync(conversationId);
            return res.ToActionResult();
        }

        [HttpGet("with/{targetUserId}")]
        public async Task<IActionResult> GetConversationWithUser(Guid targetUserId)
        {
            var res = await _conversationService.GetAsync(GetCurrentUserId(), targetUserId);
            return res.ToActionResult();
        }

        [HttpPost]
        public async Task<IActionResult> CreateGroupConversation()
        {
            var res = await _conversationService.CreateGroupAsync(
                GetCurrentUserId(),
                new List<Guid>(),
                "Test Group"
            );
            return res.ToActionResult();
        }
    }
}

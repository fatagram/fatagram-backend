using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.API.Utils;
using Fatagram.Application.Dtos.Filter;
using Fatagram.Application.Dtos.Message;
using Fatagram.Application.Services.MessageServices;
using Fatagram.Domain.Enums;
using Fatagram.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Fatagram.API.Controllers.V1
{
    public class MessageController(
        ILogger<MessageController> logger,
        IMessageService messageService
    ) : BaseApiController
    {
        private readonly ILogger<MessageController> _logger = logger;
        private readonly IMessageService _messageService = messageService;

        [HttpPost("{conversationId}")]
        public async Task<IActionResult> SendMessage(
            [FromBody] SendMessageDto request,
            Guid conversationId
        )
        {
            var createMessageRequest = new CreateMessageRequest
            {
                ConversationId = conversationId,
                Content = request.Content,
                CorrelationId = request.CorrelationId,
                ClientTempId = request.ClientTempId,
                Type = request.Type,
                Metadata = request.Metadata,
                ReceiverId = request.ReceiverId,
                Media = request.Media,
            };
            var res = await _messageService.SendMessageAsync(
                GetCurrentUserId(),
                createMessageRequest
            );
            return res.ToActionResult();
        }
    }
}

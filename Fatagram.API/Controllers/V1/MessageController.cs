using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.API.Utils;
using Fatagram.Application.Dtos.Filter;
using Fatagram.Application.Dtos.Message;
using Fatagram.Application.Services.MessageServices.Interfaces;
using Fatagram.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Fatagram.API.Controllers.V1
{
    public class MessageController : BaseApiController
    {
        private readonly ILogger<MessageController> _logger;
        private readonly IMessageService _messageService;

        public MessageController(ILogger<MessageController> logger, IMessageService messageService)
        {
            _logger = logger;
            _messageService = messageService;
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageDto request)
        {
            var res = await _messageService.SendMessageAsync(GetCurrentUserId(), request);
            return res.ToActionResult();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Fatagram.API.Controllers.V1
{
    public class MessageController : BaseApiController
    {
        private readonly ILogger<MessageController> _logger;

        public MessageController(ILogger<MessageController> logger)
        {
            _logger = logger;
        }

        public Task<IActionResult> SendFirstMessage()
        {
            throw new NotImplementedException();
        }

        public Task<IActionResult> SendMessage()
        {
            throw new NotImplementedException();
        }
    }
}

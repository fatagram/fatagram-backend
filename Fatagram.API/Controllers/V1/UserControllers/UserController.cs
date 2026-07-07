using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.API.Utils;
using Fatagram.Application.Dtos.Filter;
using Fatagram.Application.Services.UserServices.UserCoreServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Fatagram.API.Controllers.V1.UserControllers
{
    [Authorize]
    public class UserController(IUserService userService, ILogger<UserController> logger)
        : BaseApiController
    {
        private readonly ILogger<UserController> _logger = logger;
        private readonly IUserService _userService = userService;

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] CursorFilter<DateTime> filter)
        {
            var res = await _userService.GetAllAsync(filter);
            return res.ToActionResult();
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] CursorFilter<DateTime> filter)
        {
            var res = await _userService.SearchAsync(GetCurrentUserId(), filter);
            return res.ToActionResult();
        }
    }
}

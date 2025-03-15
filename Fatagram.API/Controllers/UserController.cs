using Fatagram.Application.Dtos;
using Fatagram.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Json;

namespace Fatagram_API.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService) 
        { 
            _userService = userService;
        }

        /// <summary>
        /// Get a user by username
        /// </summary>
        /// <param name="id"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetUserProfileAysnc([FromQuery]string fields)
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var res = await _userService.GetUserInfoByFieldsAsync(userId, fields);
            if (!res.IsSuccess) return BadRequest(res.ErrorCode);

            return Ok(JsonSerializer.Serialize(res.Data));
        }


        


        /// <summary>
        /// Update a user info
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPut("update-user")]
        public async Task<IActionResult> UpdateUserAsync([FromBody] UpdateUserDto request)
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var res = await _userService.UpdateUserAsync(userId, request);

            if (!res.IsSuccess) return BadRequest(res.ErrorCode);

            return Ok(res.Data);
        }
    }
}

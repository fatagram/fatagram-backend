using Fatagram.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Fatagram.API.Controllers
{
    [ApiController]
    [Route("api/other-user")]
    public class OtherUserController : ControllerBase
    {
        private readonly IOtherUserService _otherUserService;

        public OtherUserController(IOtherUserService otherUserService)
        {
            _otherUserService = otherUserService;
        }

        /// <summary>
        /// Get a user by username
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        [HttpGet("profile")]
        public async Task<IActionResult> GetOtherUserProfileAysnc([FromQuery]string userId, [FromQuery]string fields)
        {
            var res = await _otherUserService.GetOtherUserInfoByFieldsAsync(userId, fields);

            if (!res.IsSuccess) return NotFound(new
            {
                title = "Not Found",
                status = 404,
                res.ErrorCode
            });

            return Ok(JsonSerializer.Serialize(res.Data));
        }
    }
}

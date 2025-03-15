using Fatagram.Application.Dtos;
using Fatagram.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Fatagram_API.Controllers
{
    /// <summary>
    /// Controller for privacy settings
    /// </summary>
    [ApiController]
    [Route("api/privacy")]
    public class PrivacyController : ControllerBase
    {
        private readonly IPrivacySettingsService _privacySettingsService;
        public PrivacyController(IPrivacySettingsService privacySettingsService)
        {
            _privacySettingsService = privacySettingsService;
        }


        /// <summary>
        /// Get a user's privacy settings
        /// </summary>
        /// <param name="privacySettingDto"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPut("change-privacy")]
        public async Task<IActionResult> SetPrivacy([FromBody] PrivacySettingDto privacySettingDto)
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var res = await _privacySettingsService.SetPrivacySettingAsync(userId, privacySettingDto);
            if (res.IsSuccess) return Ok(res.Data);
            return BadRequest(res.ErrorCode);
        }
    }
}

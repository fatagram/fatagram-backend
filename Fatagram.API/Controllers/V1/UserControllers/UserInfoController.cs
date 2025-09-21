using Fatagram.API.Controllers.V1;
using Fatagram.API.Utils;
using Fatagram.Application.Dtos.User;
using Fatagram.Application.Dtos.User.Update;
using Fatagram.Application.Exceptions.DetailExceptions;
using Fatagram.Application.Exceptions.MiddleLevelExceptions;
using Fatagram.Shared.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Fatagram.API.Controllers.V1.UserControllers
{
    [Route("api/[controller]")]
    public class UserInfoController : BaseApiController
    {
        private readonly IUserInfoService _userInfoService;

        public UserInfoController(IUserInfoService userInfoService)
        {
            _userInfoService = userInfoService;
        }

        [Authorize]
        [HttpPatch("nickname")]
        public async Task<IActionResult> ChangeNickname([FromBody] ChangeNicknameDto changeNicknameDto)
        {
            var userId = GetCurrentUserId();
            var res = await _userInfoService.UpdateNicknameAsync(userId, changeNicknameDto);

            return res.ToActionResult();
        }

        /// <summary>
        /// Get user info overview with privacy checks.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("overview/{userId}")]
        public async Task<IActionResult> GetUserInfoOverview(Guid userId)
        {
            var uid = GetCurrentUserIdOrNull();

            // Get info with privacy check if the user is not the owner
            var res = await _userInfoService.GetUserInfoAsync(uid ?? Guid.Empty, userId);
            return res.ToActionResult();
        }
    }
}

using Fatagram.API.Utils;
using Fatagram.Application.Dtos.User;
using Fatagram.Application.Dtos.User.Update;
using Fatagram.Application.Exceptions.DetailExceptions;
using Fatagram.Application.Exceptions.MiddleLevelExceptions;
using Fatagram.Shared.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Fatagram.API.Controllers.UserControllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserInfoController : ControllerBase
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
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                throw new UnauthorizedException();
            }
            var res = await _userInfoService.UpdateNicknameAsync(userId.ToGuid(), changeNicknameDto);

            return Ok(ApiResponse<ChangeNicknameDto>.Success(
                    data: res.Data
                ));
        }

        [HttpGet("overview/{userId}")]
        public async Task<IActionResult> GetUserInfoOverview(Guid userId)
        {
            var uid = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Get info with privacy check if the user is not the owner
            var res = await _userInfoService.GetUserInfoAsync(uid.ToGuid(), userId);
            return Ok(ApiResponse<UserInfoOverview>.Success(
                    data: res.Data
                ));

        }
    }
}

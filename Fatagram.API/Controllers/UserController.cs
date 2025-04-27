using Fatagram.API.Utils;
using Fatagram.Application.Dtos.User;
using Fatagram.Application.Dtos.User.Update;
using Fatagram.Application.Exceptions;
using Fatagram.Application.Services.ImageService.Enum;
using Fatagram.Application.Services.ImageService.Interface;
using Fatagram.Application.Services.UserPrivacyServices.Interface;
using Fatagram.Application.Services.UserServices.Interface;
using Fatagram.Infrastructure.Repositories.UserPrivacyRepository.Interface;
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
        private readonly IUserPrivacyService _userPrivacyService;
        private readonly IImageService _imageService;

        public UserController(IUserService userService, IUserPrivacyService userPrivacyService, IImageService imageService) 
        { 
            _userService = userService;
            _userPrivacyService = userPrivacyService;
            _imageService = imageService;
        }

        /// <summary>
        /// Get a user by username
        /// </summary>
        /// <param name="id"></param>
        /// <param name="fields"></param>
        /// <returns></returns> 
        [HttpGet("{id}/profile")]
        public async Task<IActionResult> GetUserProfile(string id, [FromQuery]string fields)
        {
            if (HttpContext.User.Identity?.IsAuthenticated ?? false)
            {
                var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null) userId = Guid.Empty.ToString();
                var _res = await _userService.GetUserInfoAuthenticatedAsync(userId, id, fields);
                return Ok(ApiResponse<GetUserProfileDto>.Success(
                        data: _res.Data
                    ));
            }
            var res = await _userService.GetUserInfoPublicAsync(id, fields);
            return Ok(ApiResponse<GetUserProfileDto>.Success(
                    data: res.Data
                ));
        }

        /// <summary>
        /// Update a user info
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPut()]
        public async Task<IActionResult> UpdateUserAsync([FromBody] UpdateUserDto request)
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) throw new UnauthorizedException();

            var res = await _userService.UpdateUserAsync(userId, request);

            return Ok(ApiResponse<UpdateUserDto>.Success(
                    data: res.Data
                ));
        }


        [Authorize]
        [HttpPut("privacy")]
        public async Task<IActionResult> UpdateUserPrivacyAsync([FromBody] UpdateUserPrivacyDto updateUserPrivacyDto)
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) throw new UnauthorizedException();

            var res = await _userPrivacyService.UpdateUserPrivacyAsync(userId, updateUserPrivacyDto);

            return Ok(ApiResponse<string>.Success(
                    data: res.Data
                ));
        }

        [Authorize]
        [HttpPatch("avatar")]
        public async Task<IActionResult> UploadAvatarAsync(IFormFile file)
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) throw new UnauthorizedException();

            var res = await _imageService.SaveImageAsync(ImageSize.Small, file.OpenReadStream(), Path.GetExtension(file.FileName), "avatars");
            await _userService.UpdateUserAsync(userId, new UpdateUserDto()
            {
                Avatar = res.Data
            });

            return Ok(ApiResponse<string>.Success(
                    data: res.Data
                ));
        }


        [Authorize]
        [HttpPatch("background")]
        public async Task<IActionResult> UploadBackgroundAsync(IFormFile file)
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var res = await _imageService.SaveImageAsync(ImageSize.Large, file.OpenReadStream(), Path.GetExtension(file.FileName), "backgrounds");
            await _userService.UpdateUserAsync(userId, new UpdateUserDto()
            {
                Background = res.Data
            });

            return Ok(ApiResponse<string>.Success(
                   data: res.Data
                ));
        }


        [Authorize]
        [HttpPatch("url-name")]
        public async Task<IActionResult> UpdateUrlNameAsync([FromBody] ChangeUrlNameDto changeUrlNameDto)
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) throw new UnauthorizedException(); ;

            var res = await _userService.UpdateUrlNameAsync(userId, changeUrlNameDto);

            return Ok(ApiResponse<ChangeUrlNameDto>.Success(
                    data: res.Data
                ));
        }

        [Authorize]
        [HttpPatch("name")]
        public async Task<IActionResult> UpdateNameAsync([FromBody] ChangeNameDto updateNameDto)
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) throw new UnauthorizedException();

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                throw new ValidateException(errors: errors);
            }

            var res = await _userService.UpdateNameAsync(userId, updateNameDto);

            return Ok(ApiResponse<ChangeNameDto>.Success(
                    data: res.Data
                ));
        }

        [HttpGet("user-exist")]
        public async Task<IActionResult> CheckUserExist(string key)
        {
            var res = await _userService.CheckUserExistAsync(key);
            return Ok();
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) throw new UnauthorizedException();

            var res = await _userService.GetUserInfoAuthenticatedAsync(userId, userId, "id,urlName");
            return Ok(ApiResponse<object>.Success(res.Data));
        }
    }
}

using Fatagram.API.Utils;
using Fatagram.Application.Dtos.User;
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
            var hasJwt = HttpContext.User.Identity?.IsAuthenticated ?? false;
            if (fields == null) 
                return BadRequest(ApiResponse<string>.BadRequest(
                    error: new ApiError()
                    {
                        Code = new[] { "FIELDS_REQUIRED" },
                        Message = "Fields are required"
                    }
                ));

            if (hasJwt)
            {
                var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null) userId = Guid.Empty.ToString();
                var _res = await _userService.GetUserInfoAuthenticatedAsync(userId, id, fields);
                if (_res.IsSuccess) return Ok(ApiResponse<GetUserProfileDto>.Success(
                        data: _res.Data
                    ));
                return BadRequest(ApiResponse<string>.BadRequest(
                        error: new ApiError()
                        {
                            Code = new[] { _res.ErrorCode },
                            Message = "Failed to get user info"
                        }
                    ));
            }
            var res = await _userService.GetUserInfoPublicAsync(id, fields);
            if (res.IsSuccess) return Ok(ApiResponse<GetUserProfileDto>.Success(
                    data: res.Data
                ));
            return NotFound(ApiResponse<string>.NotFound(
                    error: new ApiError()
                    {
                        Code = new[] { res.ErrorCode },
                        Message = "Failed to get user info"
                    }
                ));
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
            if (!res.IsSuccess) return BadRequest(ApiResponse<string>.BadRequest(
                    error: new ApiError()
                    {
                        Code = new[] { res.ErrorCode },
                        Message = "Failed to update user"
                    }
                ));

            return Ok(ApiResponse<string>.Success(
                    data: res.Data
                ));
        }


        [Authorize]
        [HttpPut("privacy")]
        public async Task<IActionResult> UpdateUserPrivacyAsync([FromBody] UpdateUserPrivacyDto updateUserPrivacyDto)
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var res = await _userPrivacyService.UpdateUserPrivacyAsync(userId, updateUserPrivacyDto);
            if (!res.IsSuccess) return BadRequest(ApiResponse<string>.BadRequest(
                    error: new ApiError()
                    {
                        Code = new[] { res.ErrorCode },
                        Message = "Failed to update user privacy"
                    }
                ));

            return Ok(ApiResponse<string>.Success(
                    data: res.Data
                ));
        }


        [HttpPut("upload/avatar")]
        public async Task<IActionResult> UploadAvatarAsync(IFormFile file)
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var res = await _imageService.SaveImageAsync(file.OpenReadStream(), Path.GetExtension(file.FileName), "avatars");
            if (!res.IsSuccess) return BadRequest(ApiResponse<string>.BadRequest(
                    error: new ApiError()
                    {
                        Code = new[] { res.ErrorCode },
                        Message = "Failed to upload avatar"
                    }
                ));

            var res2 = await _userService.UpdateUserAsync(userId, new UpdateUserDto() { Avatar = res.Data });
            if (!res2.IsSuccess) return BadRequest(ApiResponse<string>.BadRequest(
                    error: new ApiError()
                    {
                        Code = new[] { res2.ErrorCode },
                        Message = "Failed to update user avatar"
                    }
                ));

            return Ok(ApiResponse<string>.Success(
                    data: res.Data
                ));
        }



        [Authorize]
        [HttpPut("upload/background")]
        public async Task<IActionResult> UploadBackgroundAsync(IFormFile file)
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var res = await _imageService.SaveImageAsync(file.OpenReadStream(), Path.GetExtension(file.FileName), "backgrounds");
            if (!res.IsSuccess) return BadRequest(ApiResponse<string>.BadRequest(
                    error: new ApiError()
                    {
                        Code = new[] { res.ErrorCode },
                        Message = "Failed to upload background"
                    }
                ));

            var res2 = await _userService.UpdateUserAsync(userId, new UpdateUserDto() { Background = res.Data });
            if (!res2.IsSuccess) return BadRequest(ApiResponse<string>.BadRequest(
                    error: new ApiError()
                    {
                        Code = new[] { res2.ErrorCode },
                        Message = "Failed to update user background"
                    }
                ));

            return Ok(ApiResponse<string>.Success(
                    data: res.Data
                ));
        }


        [HttpGet("user-exist")]
        public async Task<IActionResult> CheckUserExist(string key)
        {
            var res = await _userService.CheckUserExistAsync(key);
            if (res.IsSuccess) return Ok();
            return NotFound(ApiResponse<string>.NotFound(
                    error: new ApiError()
                    {
                        Code = new[] { res.ErrorCode },
                        Message = "Failed to check user exist"
                    }
                ));
        }
    }
}

using Fatagram.API.Utils;
using Fatagram.Application.Dtos.User;
using Fatagram.Application.Dtos.User.Update;
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
            var hasJwt = HttpContext.User.Identity?.IsAuthenticated ?? false;
            if (fields == null)
                return BadRequest(ApiResponse<string>.BadRequest(
                    error: new ApiError()
                    {
                        Code = new[] { "FIELDS_REQUIRED" },
                        Message = "Fields are required."
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
        [HttpPut()]
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

            return Ok(ApiResponse<UpdateUserDto>.Success(
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


        [HttpPatch("avatar")]
        public async Task<IActionResult> UploadAvatarAsync(IFormFile file)
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var res = await _imageService.SaveImageAsync(ImageSize.Small, file.OpenReadStream(), Path.GetExtension(file.FileName), "avatars");
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
        [HttpPatch("background")]
        public async Task<IActionResult> UploadBackgroundAsync(IFormFile file)
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var res = await _imageService.SaveImageAsync(ImageSize.Large, file.OpenReadStream(), Path.GetExtension(file.FileName), "backgrounds");
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


        [Authorize]
        [HttpPatch("url-name")]
        public async Task<IActionResult> UpdateUrlNameAsync([FromBody] ChangeUrlNameDto changeUrlNameDto)
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var res = await _userService.UpdateUrlNameAsync(userId, changeUrlNameDto);
            if (!res.IsSuccess) return BadRequest(ApiResponse<string>.BadRequest(
                    error: new ApiError()
                    {
                        Code = new[] { res.ErrorCode },
                        Message = res.ErrorMessage
                    }
                ));

            return Ok(ApiResponse<ChangeUrlNameDto>.Success(
                    data: res.Data
                ));
        }

        [Authorize]
        [HttpPatch("name")]
        public async Task<IActionResult> UpdateNameAsync([FromBody] ChangeNameDto updateNameDto)
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToArray();
                return BadRequest(ApiResponse<ChangeNameDto>.BadRequest(
                    error: new ApiError()
                    {
                        Code = errors,
                        Message = "Invalid input"
                    }
                ));
            }

            var res = await _userService.UpdateNameAsync(userId, updateNameDto);

            if (!res.IsSuccess) return BadRequest(ApiResponse<ChangeNameDto>.BadRequest(
                    error: new ApiError()
                    {
                        Code = new[] { res.ErrorCode },
                        Message = "Failed to update user name."
                    }
                ));

            return Ok(ApiResponse<ChangeNameDto>.Success(
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

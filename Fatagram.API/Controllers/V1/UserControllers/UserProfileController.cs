using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Fatagram.API.Controllers.V1;
using Fatagram.API.Utils;
using Fatagram.Application.Dtos.User;
using Fatagram.Application.Dtos.User.Update;
using Fatagram.Application.Exceptions;
using Fatagram.Application.Exceptions.DetailExceptions;
using Fatagram.Application.Exceptions.MiddleLevelExceptions;
using Fatagram.Application.Services.ImageService.Enum;
using Fatagram.Application.Services.UserServices.UserProfileServices.Interface;
using Fatagram.Application.Utils;
using Fatagram.Shared.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace Fatagram.API.Controllers.V1.UserControllers
{
    [Route("api/[controller]")]
    public class UserProfileController : BaseApiController
    {
        private readonly IUserProfileService _userProfileService;
        private readonly IUserPrivacyService _userPrivacyService;
        private readonly IImageService _imageService;

        public UserProfileController(
            IUserProfileService userProfileService,
            IUserPrivacyService userPrivacyService,
            IImageService imageService
        )
        {
            _userProfileService = userProfileService;
            _userPrivacyService = userPrivacyService;
            _imageService = imageService;
        }

        /// <summary>
        /// Get a user by username
        /// </summary>
        /// <param name="id"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        [HttpGet("{target}")]
        public async Task<IActionResult> GetUserProfile(string target, [FromQuery] string fields)
        {
            var userId = GetCurrentUserIdOrNull() ?? Guid.Empty;
            var res = await _userProfileService.GetUserProfileAsync(userId, target, fields);
            return res.ToActionResult();
        }

        /// <summary>
        /// Update a user info
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPut]
        public async Task<IActionResult> UpdateUserAsync([FromBody] UpdateUserDto request)
        {
            ValidateModelState();
            var userId = GetCurrentUserId();
            var res = await _userProfileService.UpdateUserAsync(userId, request);
            return res.ToActionResult();
        }

        /// <summary>
        /// Update user privacy settings
        /// </summary>
        /// <param name="updateUserPrivacyDto"></param>
        /// <returns></returns>
        /// <exception cref="UnauthorizedException"></exception>
        [Authorize]
        [HttpPut("privacy")]
        public async Task<IActionResult> UpdateUserPrivacyAsync(
            [FromBody] UpdateUserPrivacyDto updateUserPrivacyDto
        )
        {
            var userId = GetCurrentUserId();
            var res = await _userPrivacyService.UpdateUserPrivacyAsync(
                userId,
                updateUserPrivacyDto
            );

            return res.ToActionResult();
        }

        /// <summary>
        /// Upload user avatar image
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        /// <exception cref="UnauthorizedException"></exception>
        [Authorize]
        [HttpPatch("avatar")]
        public async Task<IActionResult> UploadAvatarAsync(IFormFile file)
        {
            var userId = GetCurrentUserId();
            var res = await _imageService.SaveImageAsync(
                new ImageRequest()
                {
                    ImageStream = file.OpenReadStream(),
                    Format = ImageFormat.Jpeg,
                    Quality = ImageQuality.Low,
                    Folder = $"{userId}/avatars",
                    FileName = $"avatar_{DateTime.UtcNow.Ticks}",
                }
            );
            await _userProfileService.UpdateUserAsync(
                userId,
                new UpdateUserDto() { Avatar = res.Data }
            );

            return res.ToActionResult();
        }

        /// <summary>
        /// Upload user background image
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        /// <exception cref="UnauthorizedException"></exception>
        [Authorize]
        [HttpPatch("background")]
        public async Task<IActionResult> UploadBackgroundAsync(IFormFile file)
        {
            var userId = GetCurrentUserId();
            var res = await _imageService.SaveImageAsync(
                new ImageRequest()
                {
                    ImageStream = file.OpenReadStream(),
                    Format = ImageFormat.Jpeg,
                    Quality = ImageQuality.Low,
                    Folder = $"{userId}/backgrounds",
                    FileName = $"background_{DateTime.UtcNow.Ticks}",
                }
            );
            await _userProfileService.UpdateUserAsync(
                userId,
                new UpdateUserDto() { Background = res.Data }
            );

            return res.ToActionResult();
        }

        /// <summary>
        /// Update user's URL name
        /// </summary>
        /// <param name="changeUrlNameDto"></param>
        /// <returns></returns>
        /// <exception cref="UnauthorizedException"></exception>
        [Authorize]
        [HttpPatch("urlName")]
        public async Task<IActionResult> UpdateUrlNameAsync(
            [FromBody] ChangeUrlNameDto changeUrlNameDto
        )
        {
            var userId = GetCurrentUserId();
            var res = await _userProfileService.UpdateUrlNameAsync(userId, changeUrlNameDto);
            return res.ToActionResult();
        }

        /// <summary>
        /// Update user's name
        /// </summary>
        /// <param name="updateNameDto"></param>
        /// <returns></returns>
        /// <exception cref="UnauthorizedException"></exception>
        /// <exception cref="ValidateException"></exception>
        [Authorize]
        [HttpPatch("name")]
        public async Task<IActionResult> UpdateNameAsync([FromBody] ChangeNameDto updateNameDto)
        {
            var userId = GetCurrentUserId();
            ValidateModelState();
            var res = await _userProfileService.UpdateNameAsync(userId, updateNameDto);
            return res.ToActionResult();
        }

        /// <summary>
        /// Check if a user exists by their username or email
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet("exist")]
        public async Task<IActionResult> CheckUserExist(string userId)
        {
            var res = await _userProfileService.CheckUserExistAsync(userId.ToGuid());
            return res.ToActionResult();
        }

        /// <summary>
        /// Get the authenticated user's profile information
        /// </summary>
        /// <returns></returns>
        /// <exception cref="UnauthorizedException"></exception>
        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            var userId = GetCurrentUserId();
            var res = await _userProfileService.GetUserProfileAsync(
                userId,
                userId.ToString(),
                "id,urlName,languageCode"
            );
            return res.ToActionResult();
        }
    }
}

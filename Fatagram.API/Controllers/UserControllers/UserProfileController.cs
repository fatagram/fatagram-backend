using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Fatagram.API.Utils;
using Fatagram.Application.Dtos.User;
using Fatagram.Application.Dtos.User.Update;
using Fatagram.Application.Exceptions;
using Fatagram.Application.Exceptions.DetailExceptions;
using Fatagram.Application.Exceptions.MiddleLevelExceptions;
using Fatagram.Application.Services.ImageService.Enum;
using Fatagram.Application.Services.UserServices.UserProfileServices.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace Fatagram.API.Controllers.UserControllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class UserProfileController : ControllerBase
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
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserProfile(string id, [FromQuery] string fields)
        {
            if (HttpContext.User.Identity?.IsAuthenticated ?? false)
            {
                var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null) userId = Guid.Empty.ToString();
                var _res = await _userProfileService.GetUserInfoAuthenticatedAsync(userId, id, fields);
                return Ok(ApiResponse<GetUserProfileDto>.Success(
                        data: _res.Data
                    ));
            }
            var res = await _userProfileService.GetUserInfoPublicAsync(id, fields);
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
            if (userId == null)
            {
                throw new UnauthorizedException();   
            }
            var res = await _userProfileService.UpdateUserAsync(userId, request);
            return Ok(ApiResponse<UpdateUserDto>.Success(
                    data: res.Data
                ));
        }


        [Authorize]
        [HttpPut("privacy")]
        public async Task<IActionResult> UpdateUserPrivacyAsync([FromBody] UpdateUserPrivacyDto updateUserPrivacyDto)
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                throw new UnauthorizedException();
            }
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
            if (userId == null)
            {
                throw new UnauthorizedException();
            }
            var res = await _imageService.SaveImageAsync(
                ImageSize.Small,
                file.OpenReadStream(),
                Path.GetExtension(file.FileName),
                "avatars",
                true);
            await _userProfileService.UpdateUserAsync(userId, new UpdateUserDto()
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
            if (userId == null)
            {
                throw new UnauthorizedException();
            }
            var res = await _imageService.SaveImageAsync(ImageSize.Large, file.OpenReadStream(), Path.GetExtension(file.FileName), "backgrounds");
            await _userProfileService.UpdateUserAsync(userId, new UpdateUserDto()
            {
                Background = res.Data
            });

            return Ok(ApiResponse<string>.Success(
                   data: res.Data
            ));
        }


        [Authorize]
        [HttpPatch("urlName")]
        public async Task<IActionResult> UpdateUrlNameAsync([FromBody] ChangeUrlNameDto changeUrlNameDto)
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                throw new UnauthorizedException();
            }
            var res = await _userProfileService.UpdateUrlNameAsync(userId, changeUrlNameDto);

            return Ok(ApiResponse<ChangeUrlNameDto>.Success(
                    data: res.Data
                ));
        }

        [Authorize]
        [HttpPatch("name")]
        public async Task<IActionResult> UpdateNameAsync([FromBody] ChangeNameDto updateNameDto)
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                throw new UnauthorizedException();
            }
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                throw new ValidateException("UNVALID", errors, "Unvalid data.");
            }
            var res = await _userProfileService.UpdateNameAsync(userId, updateNameDto);
            
            return res.ToActionResult();
        }

        [HttpGet("exist")]
        public async Task<IActionResult> CheckUserExist(string key)
        {
            var res = await _userProfileService.CheckUserExistAsync(key);
            return res.ToActionResult();
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) throw new UnauthorizedException();

            var res = await _userProfileService.GetUserInfoAuthenticatedAsync(userId, userId, "id,urlName");
            return res.ToActionResult();
        }
    }
}
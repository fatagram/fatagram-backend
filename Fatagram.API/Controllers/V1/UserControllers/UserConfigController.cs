using System.Security.Claims;
using Fatagram.API.Controllers.V1;
using Fatagram.API.Utils;
using Fatagram.Application.Dtos.User.Update;
using Fatagram.Application.Exceptions.DetailExceptions;
using Fatagram.Application.Services.UserServices.UserConfigServices.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fatagram.API.Controllers.V1.UserControllers
{
    [Route("api/[controller]")]
    public class UserConfigController : BaseApiController
    {
        private readonly IUserConfigService _userConfigService;

        public UserConfigController(IUserConfigService userConfigService)
        {
            _userConfigService = userConfigService;
        }

        /// <summary>
        /// Change the language of the user.
        /// </summary>
        /// <param name="changeLanguageDto"></param>
        /// <returns></returns>
        /// <exception cref="ValidateException"></exception>
        [HttpPut("language")]
        public async Task<IActionResult> ChangeLanguage(ChangeLanguageDto changeLanguageDto)
        {
            ValidateModelState();
            var userId = GetCurrentUserIdOrNull();
            if (userId == null)
            {
                return NoContent();
            }
            var res = await _userConfigService.ChangeLanguage(
                userId ?? Guid.Empty,
                changeLanguageDto.LanguageCode
            );
            return res.ToActionResult();
        }
    }
}

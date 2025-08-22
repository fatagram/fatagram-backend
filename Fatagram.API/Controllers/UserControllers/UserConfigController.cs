using Fatagram.API.Utils;
using Fatagram.Application.Dtos.User.Update;
using Fatagram.Application.Exceptions.DetailExceptions;
using Fatagram.Application.Services.UserServices.UserConfigServices.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Fatagram.API.Controllers.UserControllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserConfigController : ControllerBase
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
            if (!ModelState.IsValid)
            {
                throw new ValidateException("UNVALID", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList(), "Unvalid data");
            }

            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return NoContent();
            }

            var res = await _userConfigService.ChangeLanguage(userId, changeLanguageDto.LanguageCode);
            return res.ToActionResult<string>();
        }
    }
}

using System.Diagnostics;
using System.Security.Claims;
using Fatagram.API.Utils;
using Fatagram.Application.Dtos.Account;
using Fatagram.Application.Dtos.Auth;
using Fatagram.Application.Exceptions;
using Fatagram.Application.Exceptions.DetailExceptions;
using Fatagram.Application.Exceptions.MiddleLevelExceptions;
using Fatagram.Application.Services.AccountServices.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fatagram.API.Controllers.V1
{
    [Route("api/[controller]")]
    public class AccountController : BaseApiController
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="request">The registration details.</param>
        /// <returns>An IActionResult indicating the result of the registration.</returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto request)
        {
            ValidateModelState();
            var result = await _accountService.Register(request);
            return result.ToActionResult();
        }

        /// <summary>
        /// Changes the password of the authenticated user.
        /// </summary>
        /// <param name="request">The change password details.</param>
        /// <returns>An IActionResult indicating the result of the password change.</returns>
        [Authorize]
        [HttpPut("changePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto request)
        {
            var userId = GetCurrentUserId();
            var result = await _accountService.ChangePasswordAsync(userId, request);
            return result.ToActionResult();
        }
    }
}

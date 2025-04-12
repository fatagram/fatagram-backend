using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using Fatagram.API.Utils;
using Fatagram.Application.Services.AccountServices.Interface;
using Fatagram.Application.Dtos.Auth;
using Fatagram.Application.Dtos.Account;

namespace Fatagram.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
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
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToArray();
                return BadRequest(ApiResponse<string>.BadRequest(
                        error: new ApiError()
                        {
                            Code = errors,
                            Message = "Invalid input"
                        }
                    ));
            }
            var result = await _accountService.Register(request);
            return result.IsSuccess ?
                Ok(ApiResponse<string>.Success(
                    data: result.Data,
                    message: "Register successfully"
                )) :
                BadRequest(ApiResponse<string>.BadRequest(
                        error: new ApiError()
                        {
                            Code = new[] { result.ErrorCode },
                            Message = "Register failed"
                        }
                    ));
        }

        /// <summary>
        /// Changes the password of the authenticated user.
        /// </summary>
        /// <param name="request">The change password details.</param>
        /// <returns>An IActionResult indicating the result of the password change.</returns>
        [Authorize]
        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto request)
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var result = await _accountService.ChangePasswordAsync(userId, request);
            return result.IsSuccess ?
                Ok(ApiResponse<string>.Success(
                    data: result.Data,
                    message: "Change password successfully"
                    )) :
                BadRequest(ApiResponse<string>.BadRequest(
                    error: new ApiError()
                    {
                        Code = new[] { result.ErrorCode },
                        Message = "Change password failed"
                    }
                ));
        }
    }
}

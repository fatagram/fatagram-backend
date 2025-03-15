using Fatagram.Application.Dtos;
using Fatagram.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using Fatagram.API.Utils;

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
        /// Login a user
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToArray();

                return BadRequest(new ApiResponse<string>(
                    "404",
                    new ApiError() 
                    {
                        Code = errors,
                        Message = new[] { "Register failed" }
                    })
                );
            }

            var result = await _accountService.Register(request);
            return result.IsSuccess ? Ok(new ApiResponse<string>("200", result.Data, "Register success")) :
                BadRequest(new ApiResponse<string>("400", new ApiError()
                {
                    Code = new[] { result.ErrorCode },
                    Message = new[] { "Register failed" }
                }));
        }



        /// <summary>
        /// Change password
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// 
        [Authorize]
        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto request)
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            Debug.WriteLine("User ID: " + userId);
            if (userId == null) return Unauthorized();

            var result = await _accountService.ChangePasswordAsync(userId, request);
            return result.IsSuccess ? Ok(result.Data) 
                : BadRequest(new ApiResponse<string>(
                        status: "400",
                        error: new ApiError()
                        {
                            Code = new[] { result.ErrorCode },
                            Message = new[] { "Change password failed" }
                        }
                    ));
        }

    }
}

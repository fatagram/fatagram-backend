
using Fatagram.API.Utils;
using Fatagram.Application.Dtos;
using Fatagram.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security.Claims;

namespace Fatagram.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ITokenService _tokenService;
        // private readonly IUserService _userService;

        public AuthController(IAuthService authService, ITokenService tokenService)
        {
            _authService = authService;
            _tokenService = tokenService;
            // _userService = userService;
        }



        /// <summary>
        /// Login a user
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToArray();
                return BadRequest(new ApiResponse<string>(
                        status: "400",
                        error: new ApiError()
                        {
                            Code = errors,
                        }
                    ));
            }

            var result = await _authService.Login(request);
            if (result.IsSuccess)
            {
                var res = await _tokenService.GenerateTokensAsync(request.Username);
                if (!res.IsSuccess || res.Data == null)
                {
                    return BadRequest(new { 
                        status = 400,
                        code = new[] { res.ErrorCode }
                    });
                }
                return Ok(new TokenDto()
                    {
                        AccessToken = res.Data.AccessToken,
                        RefreshToken = res.Data.RefreshToken
                    }
                );
            }
            return BadRequest(new
            {
                status = 400,
                code = new[] { result.ErrorCode }
            });
        }


        /// <summary>
        /// Refresh token
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto request)
        {
            var res = await _tokenService.RefreshAccessTokenAsync(request.RefreshToken);
            if (!res.IsSuccess || res.Data == null)
            {
                await _tokenService.DeleteRefreshTokenAsync(request.RefreshToken);
                return BadRequest(new
                {
                    status = 400,
                    code = new[] { res.ErrorCode }
                });
            }
            return Ok(new TokenDto() { AccessToken = res.Data });
        }


        /// <summary>
        /// Ping access token
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet("ping-access-token")]
        public IActionResult PingAccessToken()
        {
            return NoContent();
        }
    }
}

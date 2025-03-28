using Fatagram.API.Utils;
using Fatagram.Application.Dtos.Auth;
using Fatagram.Application.Dtos.Token;
using Fatagram.Application.Services.AuthServices.Interface;
using Fatagram.Application.Services.TokenServices.Interface;
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
                return BadRequest(ApiResponse<string>.BadRequest(
                    error: new ApiError()
                    {
                        Code = errors,
                        Message = "Invalid input"
                    }
                ));
            }
            var result = await _authService.Login(request);
            if (result.IsSuccess)
            {
                var res = await _tokenService.GenerateTokensAsync(request.Username);
                if (!res.IsSuccess || res.Data == null)
                {
                    return BadRequest(ApiResponse<string>.BadRequest(
                        error: new ApiError()
                        {
                            Code = new[] { res.ErrorCode },
                        }
                    ));
                }

                Response.Cookies.Append("accessToken", res.Data.AccessToken, new Microsoft.AspNetCore.Http.CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = Microsoft.AspNetCore.Http.SameSiteMode.None,
                    MaxAge = TimeSpan.FromMinutes(60)
                });

                return Ok(ApiResponse<TokenDto>.Success(
                        data: new TokenDto() { RefreshToken = res.Data.RefreshToken },
                        message: "Login successfully"
                    ));
            }
            return BadRequest(ApiResponse<string>.BadRequest(
                error: new ApiError
                {
                    Code = new[] { result.ErrorCode }
                }));
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
                return BadRequest(ApiResponse<string>.BadRequest(
                    error:new ApiError()
                    {
                        Code = new[] { res.ErrorCode }
                    }
                ));
            }
            var cookieOptions = new Microsoft.AspNetCore.Http.CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = Microsoft.AspNetCore.Http.SameSiteMode.None,
                MaxAge = TimeSpan.FromMinutes(60)
            };
            Response.Cookies.Append("accessToken", res.Data, cookieOptions);

            return Ok(ApiResponse<TokenDto>.Success(
                    data: new TokenDto() { RefreshToken = request.RefreshToken }
                ));
        }

        /// <summary>
        /// Ping access token
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return NoContent();
        }
    }
}

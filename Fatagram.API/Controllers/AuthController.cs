using Fatagram.API.Utils;
using Fatagram.Application.Dtos.Auth;
using Fatagram.Application.Dtos.Token;
using Fatagram.Application.Exceptions;
using Fatagram.Application.Services.AuthServices.Interface;
using Fatagram.Application.Services.TokenServices.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                throw new ValidateException(errors: errors);
            }
            var result = await _authService.Login(request);
            var res = await _tokenService.GenerateTokensAsync(request.Username);
            if (res.Data == null)
            {
                throw new DataNullException();
            }

            Response.Cookies.Append("accessToken", res.Data.AccessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                MaxAge = TimeSpan.FromMinutes(60)
            });

            if (result.Data is not null)
                result.Data.RefreshToken = res.Data.RefreshToken;

            return Ok(ApiResponse<LoginResponseDto>.Success(
                    data: result.Data
                ));
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
            //if (!res.IsSuccess || res.Data == null)
            //{
            //    await _tokenService.DeleteRefreshTokenAsync(request.RefreshToken);

            //}
            if (res.Data is null) throw new DataNullException();
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                MaxAge = TimeSpan.FromMinutes(60)
            };
            Response.Cookies.Append("accessToken", res.Data, cookieOptions);

            return Ok(ApiResponse<TokenDto>.Success(
                    data: new TokenDto() { RefreshToken = request.RefreshToken }
                ));
        }


        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] RefreshTokenRequestDto request)
        {
            var res = await _tokenService.DeleteRefreshTokenAsync(request.RefreshToken);
            Response.Cookies.Append("accessToken", "", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.Now.AddDays(-1)
            });
            return Ok(ApiResponse<string>.Success());
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

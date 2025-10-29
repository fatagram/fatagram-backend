using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using Fatagram.API.Extensions.Constrains;
using Fatagram.API.Utils;
using Fatagram.Application.Dtos.Auth;
using Fatagram.Application.Dtos.Token;
using Fatagram.Application.Exceptions;
using Fatagram.Application.Exceptions.DetailExceptions;
using Fatagram.Application.Services.AuthServices.Interface;
using Fatagram.Application.Services.TokenServices.Interface;
using Fatagram.Application.Utils;
using Fatagram.Shared.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fatagram.API.Controllers.V1
{
    [Route("api/[controller]")]
    public class AuthController : BaseApiController
    {
        private readonly IAuthService _authService;
        private readonly ITokenService _tokenService;

        // private readonly IUserService _userService;

        public AuthController(IAuthService authService, ITokenService tokenService)
        {
            _authService = authService;
            _tokenService = tokenService;
        }

        /// <summary>
        /// Login a user
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto request)
        {
            ValidateModelState();
            var res = await _authService.Login(request);
            if (!res.IsSuccess)
            {
                return res.ToActionResult();
            }
            var generateTokenResult = await _tokenService.GenerateTokensAsync(request.Username);
            if (generateTokenResult.Data == null)
            {
                throw new DataNullException("TOKEN_CANNOT_CREATE", "Token cannot be created");
            }

            // Create new cookie with access token
            Response.Cookies.Append(
                "accessToken",
                generateTokenResult.Data.AccessToken,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = _isSecureCookies,
                    SameSite = Setups.IsForLAN ? SameSiteMode.Lax : SameSiteMode.None,
                    MaxAge = TimeSpan.FromMinutes(60),
                }
            );

            if (res.Data is not null)
                res.Data.RefreshToken = generateTokenResult.Data.RefreshToken;

            return res.ToActionResult();
        }

        /// <summary>
        /// Refresh token
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("refreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto request)
        {
            var res = await _tokenService.RefreshAccessTokenAsync(request.RefreshToken);
            //if (!res.IsSuccess || res.Data == null)
            //{
            //    await _tokenService.DeleteRefreshTokenAsync(request.RefreshToken);

            //}
            if (res.Data is null)
                throw new DataNullException();
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = _isSecureCookies,
                SameSite = Setups.IsForLAN ? SameSiteMode.Lax : SameSiteMode.None,
                MaxAge = TimeSpan.FromMinutes(60),
            };
            Response.Cookies.Append("accessToken", res.Data, cookieOptions);

            return res.ToActionResult();
        }

        /// <summary>
        /// Logout a user
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] RefreshTokenRequestDto request)
        {
            if (request.RefreshToken != null)
                await _tokenService.DeleteRefreshTokenAsync(request.RefreshToken);
            Response.Cookies.Append(
                "accessToken",
                "",
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = _isSecureCookies,
                    SameSite = Setups.IsForLAN ? SameSiteMode.Lax : SameSiteMode.None,
                    Expires = DateTime.Now.AddDays(-1),
                }
            );
            return Ok();
        }

        /// <summary>
        /// Ping access token
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok();
        }
    }
}

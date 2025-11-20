using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using Fatagram.API.Extensions.Constrains;
using Fatagram.API.Utils;
using Fatagram.Application.Dtos.Account;
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
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            IAuthService authService,
            ITokenService tokenService,
            ILogger<AuthController> logger
        )
        {
            _authService = authService;
            _tokenService = tokenService;
            _logger = logger;
        }

        /// <summary>
        /// Login a user
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto request)
        {
            var res = await _authService.Login(request);
            this._logger.LogInformation("Token: {Token}", res);
            Response.Cookies.Append(
                "_accessToken",
                res.Data!.AccessToken,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = false,
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTime.Now.AddMinutes(30),
                }
            );
            Response.Cookies.Append(
                "_refreshToken",
                res.Data!.RefreshToken,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = false,
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTime.Now.AddDays(7),
                }
            );
            return Ok();
        }

        /// <summary>
        /// Refresh token
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("refreshToken")]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["_refreshToken"]!;
            var res = await _tokenService.GenerateAccessTokenFromRefreshTokenAsync(refreshToken);
            Response.Cookies.Append(
                "_accessToken",
                res!,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = false,
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTime.Now.AddMinutes(30),
                }
            );
            return Ok();
        }

        /// <summary>
        /// Logout a user
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            if (
                !Request.Cookies.TryGetValue("_refreshToken", out var token)
                || string.IsNullOrWhiteSpace(token)
            )
                await _tokenService.DeleteRefreshTokenAsync(token!);

            Response.Cookies.Append(
                "_accessToken",
                "",
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = false,
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTime.Now.AddDays(-1),
                }
            );
            Response.Cookies.Append(
                "_refreshToken",
                "",
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = false,
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTime.Now.AddDays(-1),
                }
            );
            return Ok();
        }

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="request">The registration details.</param>
        /// <returns>An IActionResult indicating the result of the registration.</returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto request)
        {
            var result = await _authService.Register(request);
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
            var result = await _authService.ChangePasswordAsync(userId, request);
            return result.ToActionResult();
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

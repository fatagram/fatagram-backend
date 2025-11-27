using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using Fatagram.API.Extensions.Constrains;
using Fatagram.API.Utils;
using Fatagram.API.Utils.Attributes;
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
using Microsoft.Identity.Client;

namespace Fatagram.API.Controllers.V1
{
    [Route("api/[controller]")]
    public class AuthController(
        IAuthService authService,
        ITokenService tokenService,
        ILogger<AuthController> logger
    ) : BaseApiController
    {
        private readonly IAuthService _authService = authService;
        private readonly ITokenService _tokenService = tokenService;
        private readonly ILogger<AuthController> _logger = logger;

        /// <summary>
        /// Login a user
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto request)
        {
            var res = await _authService.Login(request);
            AppendAccessToken(res.Data!.AccessToken);
            AppendRefreshToken(res.Data!.RefreshToken);
            return Ok();
        }

        [HttpPost("google/callback")]
        public async Task<IActionResult> GoogleCallback([FromBody] GoogleCallbackDto request)
        {
            _logger.LogInformation("Google callback: {request}", request.Code);
            var res = await _authService.GoogleCallback(request);
            AppendAccessToken(res.Data!.AccessToken);
            AppendRefreshToken(res.Data!.RefreshToken);
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
            if (!Request.Cookies.TryGetValue("_refreshToken", out var refreshToken))
            {
                throw new GenerateTokenException();
            }
            var res = await _tokenService.GenerateAccessTokenFromRefreshTokenAsync(refreshToken);
            AppendAccessToken(res!);
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

            RemoveAccessToken();
            RemoveRefreshToken();
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
        [NotRequireOnBoarding]
        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok();
        }

        private void RemoveAccessToken()
        {
            Response.Cookies.Delete("_accessToken");
        }

        private void RemoveRefreshToken()
        {
            Response.Cookies.Delete("_refreshToken");
        }

        private void AppendAccessToken(string value)
        {
            Response.Cookies.Append(
                "_accessToken",
                value,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = false,
                    SameSite = SameSiteMode.Lax,
                    Path = "/",
                    Expires = DateTime.Now.AddMinutes(30),
                }
            );
        }

        private void AppendRefreshToken(string value)
        {
            Response.Cookies.Append(
                "_refreshToken",
                value,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = false,
                    SameSite = SameSiteMode.Lax,
                    Path = "/",
                    Expires = DateTime.Now.AddDays(7),
                }
            );
        }
    }
}

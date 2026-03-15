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

namespace Fatagram.API.Controllers.V1
{
    public class AuthController(
        IAuthService authService,
        ITokenService tokenService,
        ILogger<AuthController> logger,
        IWebHostEnvironment environment,
        IConfiguration configuration
    ) : BaseApiController
    {
        private readonly IAuthService _authService = authService;
        private readonly ITokenService _tokenService = tokenService;
        private readonly ILogger<AuthController> _logger = logger;
        private readonly IWebHostEnvironment _environment = environment;
        private readonly IConfiguration _configuration = configuration;

        private bool IsLocal => !(_environment.IsProduction() || _environment.IsDevelopment());

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

        /// <summary>
        /// OAuth callback - Generic OAuth authentication endpoint
        /// </summary>
        /// <param name="provider">OAuth provider (google, facebook, github, microsoft)</param>
        /// <param name="request">OAuth callback request with authorization code</param>
        /// <returns></returns>
        [HttpPost("oauth/{provider}/callback")]
        public async Task<IActionResult> OAuthCallback(
            string provider,
            [FromBody] OAuthCallbackDto request
        )
        {
            _logger.LogInformation(
                "Received OAuth callback for provider: {Provider} with code: {Code}",
                provider,
                request.Code
            );
            if (!Enum.TryParse<OAuthProvider>(provider, ignoreCase: true, out var oauthProvider))
            {
                return BadRequest($"Unsupported OAuth provider: {provider}");
            }

            _logger.LogInformation("{Provider} OAuth callback: {code}", provider, request.Code);
            var res = await _authService.OAuthCallback(oauthProvider, request.Code);
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
        [HttpPut("password")]
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
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = !IsLocal,
                SameSite = IsLocal ? SameSiteMode.Lax : SameSiteMode.None,
                Domain = _configuration["Domain"],
                Expires = DateTime.Now.AddDays(-1),
            };
            Response.Cookies.Delete("_accessToken", cookieOptions);
        }

        private void RemoveRefreshToken()
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = !IsLocal,
                SameSite = IsLocal ? SameSiteMode.Lax : SameSiteMode.None,
                Domain = _configuration["Domain"],
                Expires = DateTime.Now.AddDays(-1),
            };
            Response.Cookies.Delete("_refreshToken", cookieOptions);
        }

        private void AppendAccessToken(string value)
        {
            Response.Cookies.Append(
                "_accessToken",
                value,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = !IsLocal,
                    SameSite = IsLocal ? SameSiteMode.Lax : SameSiteMode.None,
                    Domain = _configuration["Domain"],
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
                    Secure = !IsLocal,
                    SameSite = IsLocal ? SameSiteMode.Lax : SameSiteMode.None,
                    Path = "/",
                    Domain = _configuration["Domain"],
                    Expires = DateTime.Now.AddDays(7),
                }
            );
        }
    }
}

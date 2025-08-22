using Fatagram.API.Extensions.Constrains;
using Fatagram.API.Utils;
using Fatagram.Application.Dtos.Auth;
using Fatagram.Application.Dtos.Token;
using Fatagram.Application.Exceptions;
using Fatagram.Application.Exceptions.DetailExceptions;
using Fatagram.Application.Services.AuthServices.Interface;
using Fatagram.Application.Services.TokenServices.Interface;
using Fatagram.Shared.Enums;
using Fatagram.Application.Utils;
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

        private bool _isSecureCookies = !Setups.IsForLAN;

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
            // Validate the request model
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                throw new ValidateException("UNVALID", errors, "Unvalid data");
            }
            var loginResult = await _authService.Login(request);
            if (!loginResult.IsSuccess)
            {
                return loginResult.ToActionResult();
            }
            var generateResult = await _tokenService.GenerateTokensAsync(request.Username);
            if (generateResult.Data == null)
            {
                throw new DataNullException("TOKEN_CANNOT_CREATE", "Token cannot be created");
            }
            
            // Create new cookie with access token
            Response.Cookies.Append("accessToken", generateResult.Data.AccessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = _isSecureCookies,
                SameSite = Setups.IsForLAN ? SameSiteMode.Lax : SameSiteMode.None,
                MaxAge = TimeSpan.FromMinutes(60)
            });

            if (loginResult.Data is not null)
                loginResult.Data.RefreshToken = generateResult.Data.RefreshToken;

            return loginResult.ToActionResult();
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
            if (res.Data is null) throw new DataNullException();
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = _isSecureCookies,
                SameSite = Setups.IsForLAN ? SameSiteMode.Lax : SameSiteMode.None,
                MaxAge = TimeSpan.FromMinutes(60)
            };
            Response.Cookies.Append("accessToken", res.Data, cookieOptions);

            return Ok(ApiResponse<TokenDto>.Success(
                    data: new TokenDto() { RefreshToken = request.RefreshToken }
                ));
        }

        /// <summary>
        /// Logout a user
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] RefreshTokenRequestDto request)
        {
            try
            {
                if (request.RefreshToken != null) await _tokenService.DeleteRefreshTokenAsync(request.RefreshToken);
            }
            catch(Exception) { }
            Response.Cookies.Append("accessToken", "", new CookieOptions
            {
                HttpOnly = true,
                Secure = _isSecureCookies,
                SameSite = Setups.IsForLAN ? SameSiteMode.Lax : SameSiteMode.None,
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
            return Ok();
        }
    }
}

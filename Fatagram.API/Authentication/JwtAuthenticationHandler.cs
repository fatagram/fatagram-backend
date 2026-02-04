using System.Diagnostics;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Fatagram.Application.Services.JwtServices.Interface;
using Fatagram.Application.Utils;
using Fatagram.Shared.Enums;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.Extensions.Options;

namespace Fatagram.API.Authentication
{
    /// <summary>
    /// Custom authentication handler for JWT
    /// </summary>
    public class JwtAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory loggerFactory,
        UrlEncoder encoder,
        IJwtService jwtService,
        ILogger<JwtAuthenticationHandler> logger
    ) : AuthenticationHandler<AuthenticationSchemeOptions>(options, loggerFactory, encoder)
    {
        private readonly IJwtService _jwtService = jwtService;
        private readonly ILogger<JwtAuthenticationHandler> _logger = logger;

        /// <summary>
        /// Handle authentication
        /// </summary>
        /// <returns></returns>
        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            string? token = null;
            if (Request.Headers.TryGetValue("Authorization", out var authHeaderValues))
            {
                var authHeader = authHeaderValues.ToString();
                if (authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    token = authHeader.Substring("Bearer ".Length).Trim();
                }
            }

            if (string.IsNullOrEmpty(token))
            {
                Request.Cookies.TryGetValue("_accessToken", out token);
            }

            if (string.IsNullOrEmpty(token))
            {
                return Task.FromResult(AuthenticateResult.NoResult());
            }

            try
            {
                var res = _jwtService.ValidateToken(token!);
                if (res == null || res.Data == null || res.Code == ResponseStatusCode.Unauthorized)
                {
                    return Task.FromResult(AuthenticateResult.Fail("Invalid token"));
                }

                var ticket = new AuthenticationTicket(res.Data!, Scheme.Name);
                return Task.FromResult(AuthenticateResult.Success(ticket));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while validating token");
                return Task.FromResult(AuthenticateResult.Fail("Invalid token"));
            }
        }
    }
}

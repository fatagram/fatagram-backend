using System.Diagnostics;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Fatagram.Application.Services.JwtServices.Interface;
using Fatagram.Application.Utils;
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
            if (
                !Request.Cookies.TryGetValue("_accessToken", out var token)
                || string.IsNullOrWhiteSpace(token)
            )
            {
                return Task.FromResult(AuthenticateResult.Fail("Missing Authorization Header"));
            }

            try
            {
                var res = _jwtService.ValidateToken(token);
                var ticket = new AuthenticationTicket(res.Data!, "JwtCustomScheme");

                return Task.FromResult(AuthenticateResult.Success(ticket));
            }
            catch (Exception ex)
            {
                return Task.FromResult(
                    AuthenticateResult.Fail($"Token validation failed: {ex.Message}")
                );
            }
        }
    }
}

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
    public class JwtAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly IJwtService _jwtService;

        public JwtAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            IJwtService jwtService
        ) // Inject JwtService
            : base(options, logger, encoder)
        {
            _jwtService = jwtService;
        }

        /// <summary>
        /// Handle authentication
        /// </summary>
        /// <returns></returns>
        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // Get token from header
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

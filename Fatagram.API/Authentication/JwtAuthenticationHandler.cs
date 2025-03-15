using Fatagram.Application.Services.Interfaces;
using Fatagram.Application.Utils;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Text.Encodings.Web;

namespace Fatagram.API.Authentication
{
    /// <summary>
    /// Custom authentication handler for JWT
    /// </summary>
    public class JwtAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly IJwtService _jwtService;

        private readonly TimeProvider _clock;

        public JwtAuthenticationHandler(
                        IOptionsMonitor<AuthenticationSchemeOptions> options,
                        ILoggerFactory logger,
                        UrlEncoder encoder,
                        TimeProvider clock,
                        IJwtService jwtService) // Inject JwtService
        : base(options, logger, encoder)
        {
            _jwtService = jwtService;
            _clock = clock;
        }



        /// <summary>
        /// Handle authentication
        /// </summary>
        /// <returns></returns>
        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // Get token from header
            if (!Request.Headers.ContainsKey("Authorization"))
                return Task.FromResult(AuthenticateResult.Fail("Missing Authorization Header"));

            // Get token from header
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "").Trim();
            Debug.WriteLine($"Token: {token}");

            try
            {
                var res = _jwtService.ValidateToken(token);
                if (!res.IsSuccess || res.Data == null)
                {
                    Debug.WriteLine("Invalid token");
                    return Task.FromResult(AuthenticateResult.Fail(ErrorCodes.TOKEN_INVALID));
                }

                var ticket = new AuthenticationTicket(res.Data, "JwtCustomScheme");

                Debug.WriteLine("Token is valid");
                return Task.FromResult(AuthenticateResult.Success(ticket));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Token validation failed: {ex.Message}");
                return Task.FromResult(AuthenticateResult.Fail($"Token validation failed: {ex.Message}"));
            }
        }
    }
}

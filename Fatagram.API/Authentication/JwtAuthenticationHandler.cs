using System.Text.Encodings.Web;
using Fatagram.Application.Abstractions.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace Fatagram.API.Authentication
{
    public class JwtAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory loggerFactory,
        UrlEncoder encoder,
        IJwtTokenValidator jwtTokenValidator,
        ILogger<JwtAuthenticationHandler> logger
    ) : AuthenticationHandler<AuthenticationSchemeOptions>(options, loggerFactory, encoder)
    {
        private readonly IJwtTokenValidator _jwtTokenValidator = jwtTokenValidator;
        private readonly ILogger<JwtAuthenticationHandler> _logger = logger;

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var token = ExtractToken();

            if (string.IsNullOrEmpty(token))
                return Task.FromResult(AuthenticateResult.NoResult());

            try
            {
                var principal = _jwtTokenValidator.ValidateToken(token);
                if (principal is null)
                    return Task.FromResult(AuthenticateResult.Fail("Invalid token"));

                var ticket = new AuthenticationTicket(principal, Scheme.Name);
                return Task.FromResult(AuthenticateResult.Success(ticket));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while validating token");
                return Task.FromResult(AuthenticateResult.Fail("Invalid token"));
            }
        }

        private string? ExtractToken()
        {
            if (Request.Headers.TryGetValue("Authorization", out var authHeaderValues))
            {
                var authHeader = authHeaderValues.ToString();
                if (authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                    return authHeader["Bearer ".Length..].Trim();
            }

            Request.Cookies.TryGetValue("_accessToken", out var cookieToken);
            return cookieToken;
        }
    }
}

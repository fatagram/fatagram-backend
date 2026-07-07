using Fatagram.API.Authentication;
using Fatagram.Application.Types;

namespace Fatagram.API.Extensions.Services
{
    public static class AuthExtensions
    {
        public static void AddAuthenticationServices(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
            services
                .AddAuthentication("JwtAuthenticationScheme")
                .AddScheme<AuthenticationSchemeOptions, JwtAuthenticationHandler>(
                    "JwtAuthenticationScheme",
                    null
                );
        }
    }
}

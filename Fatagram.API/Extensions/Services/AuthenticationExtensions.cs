using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fatagram.API.Extensions.Services
{
    public static class AuthenticationExtensions
    {
        public static void AddAuthenticationServices(this IServiceCollection services)
        {
            // Add authentication
            services
                .AddAuthentication("JwtAuthenticationScheme")
                .AddScheme<AuthenticationSchemeOptions, JwtAuthenticationHandler>(
                    "JwtAuthenticationScheme",
                    null
                );
        }
    }
}

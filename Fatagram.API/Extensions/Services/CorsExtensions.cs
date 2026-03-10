using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.API.Extensions.Constrains;

namespace Fatagram.API.Extensions.Services
{
    public static class CorsExtensions
    {
        public static void AddCorsServices(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(
                    name: CorsPolicySettings.MyAllowSpecificOrigins,
                    builder =>
                    {
                        var domain =
                            Environment.GetEnvironmentVariable("DOMAIN") ?? "localhost:3000";
                        var allowedUrl =
                            domain == "localhost" ? "http://localhost:3000" : $"https://{domain}";
                        builder
                            .WithOrigins(allowedUrl)
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials();
                    }
                );
            });
        }
    }
}

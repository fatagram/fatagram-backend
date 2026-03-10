using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.API.Extensions.Constrains;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Web;

namespace Fatagram.API.Extensions.Services
{
    public static class CorsExtensions
    {
        public static void AddCorsServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(
                    name: CorsPolicySettings.MyAllowSpecificOrigins,
                    policy =>
                    {
                        var allowedUrl = (builder.Configuration["AllowedOrigin"] ?? "")
                            .Split(",")
                            .Select(url => url.Trim())
                            .ToArray();
                        policy
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

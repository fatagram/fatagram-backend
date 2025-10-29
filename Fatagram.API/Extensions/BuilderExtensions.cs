using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.API.Extensions.Configuration;
using Fatagram.API.Extensions.Hosting;
using Fatagram.API.Extensions.Services;

namespace Fatagram.API.Extensions
{
    public static class BuilderExtensions
    {
        public static void ConfigureAppConfiguration(this WebApplicationBuilder builder)
        {
            // Configure services
            builder.AddAppConfiguration();
            builder.AddApplicationServices();
            builder.ConfigureKestrel();

            if (!int.TryParse(builder.Configuration["MaxSize"], out var imageMaxSize))
            {
                imageMaxSize = 20; // default 2MB
            }

            // Max request body size
            builder.WebHost.UseKestrel(option =>
            {
                option.Limits.MaxRequestBodySize = imageMaxSize * 1024 * 1024; // 2MB
            });
        }
    }
}

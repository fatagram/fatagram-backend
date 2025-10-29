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
                        builder
                            .SetIsOriginAllowed(origin => true)
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials();
                    }
                );
            });
        }
    }
}

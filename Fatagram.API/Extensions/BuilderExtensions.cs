using Fatagram.API.Extensions.Configuration;
using Fatagram.API.Extensions.Services;

namespace Fatagram.API.Extensions
{
    public static class BuilderExtensions
    {
        public static void ConfigureAppConfiguration(this WebApplicationBuilder builder)
        {
            builder.AddAppConfiguration();
            builder.AddApplicationServices();

            if (!int.TryParse(builder.Configuration["MaxSize"], out var imageMaxSize))
            {
                imageMaxSize = 20;
            }

            builder.WebHost.UseKestrel(option =>
            {
                option.Limits.MaxRequestBodySize = imageMaxSize * 1024 * 1024;
            });
        }
    }
}

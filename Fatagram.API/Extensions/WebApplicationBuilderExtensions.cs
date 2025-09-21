using System.Net;
using System.Runtime.CompilerServices;

namespace Fatagram.API.Extensions
{
    public static class WebApplicationBuilderExtensions
    {
        public static void ConfigureKestrelOptions(this WebApplicationBuilder builder)
        {
            //var certPath = builder.Configuration["PfxPath"] ?? "";
            //var certPassword = builder.Configuration["PfxPassword"];

            // Add authorization
            builder.WebHost.ConfigureKestrel(options =>
            {
                options.ListenAnyIP(5000);
                //options.ListenLocalhost(5001, listenOptions =>
                //{
                //    listenOptions.UseHttps(certPath, certPassword);
                //});
            });
        }
    }
}

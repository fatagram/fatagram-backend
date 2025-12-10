using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fatagram.API.Extensions.Hosting
{
    public static class HostingExtension
    {
        public static void ConfigureKestrel(this WebApplicationBuilder builder)
        {
            //var certPath = builder.Configuration["PfxPath"] ?? "";
            //var certPassword = builder.Configuration["PfxPassword"];

            // Add authorization
            // builder.WebHost.ConfigureKestrel(options =>
            // {
            //     options.ListenLocalhost(5000);
            //     //options.ListenLocalhost(5001, listenOptions =>
            //     //{
            //     //    listenOptions.UseHttps(certPath, certPassword);
            //     //});
            // });

            builder.WebHost.UseKestrel();
        }
    }
}

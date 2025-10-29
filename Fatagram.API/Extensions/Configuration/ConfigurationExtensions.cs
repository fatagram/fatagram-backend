using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fatagram.API.Extensions.Configuration
{
    public static class ConfigurationExtensions
    {
        public static void AddAppConfiguration(this WebApplicationBuilder builder)
        {
            builder
                .Configuration.AddJsonFile(
                    "appsettings.json",
                    optional: false,
                    reloadOnChange: true
                )
                .AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true);
        }
    }
}

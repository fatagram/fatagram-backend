using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Fatagram.API.Extensions.Services
{
    public static class ControllerExtensions
    {
        public static void AddControllerServices(this IServiceCollection services)
        {
            services.AddControllers();
            services
                .AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(null));
                });
        }
    }
}

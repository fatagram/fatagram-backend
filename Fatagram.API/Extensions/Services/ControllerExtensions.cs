using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Fatagram.API.Utils;

namespace Fatagram.API.Extensions.Services
{
    public static class ControllerExtensions
    {
        public static void AddControllerServices(this IServiceCollection services)
        {
            services
                .AddControllers(options =>
                {
                    options.Filters.Add<ValidationFilter>();
                })
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(null));
                });
        }
    }
}

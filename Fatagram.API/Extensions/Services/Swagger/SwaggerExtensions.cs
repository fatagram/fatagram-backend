using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fatagram.API.Extensions.Services.Swagger
{
    public static class SwaggerExtensions
    {
        public static void AddSwaggerServices(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
        }
    }
}

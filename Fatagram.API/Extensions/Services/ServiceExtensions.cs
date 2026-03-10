using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.API.Extensions.Services.Swagger;
using Fatagram.API.Extensions.SignalR;
using Fatagram.Application.Validators.Auth;
using FluentValidation;

namespace Fatagram.API.Extensions.Services
{
    public static class ServiceExtensions
    {
        public static void AddApplicationServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddHttpContextAccessor();

            // Turn off ModelStateInvalidFilter
            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            // Infrastructure Services
            builder.Services.AddDatabaseServices(builder.Configuration);
            builder.Services.AddDependencyServices();
            builder.Services.AddCloudStorageServices(builder.Configuration);

            // Framework Services
            builder.Services.AddAuthenticationServices();
            builder.AddCorsServices();
            builder.Services.AddSignalRServices();

            // Presentation Services
            builder.Services.AddControllerServices();
            builder.Services.AddApiVersioningServices();
            builder.Services.AddSwaggerServices();

            // FluentValidation
            builder.Services.AddValidatorsFromAssemblyContaining<RegisterDtoValidator>();
        }
    }
}

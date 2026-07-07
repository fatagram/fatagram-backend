using Fatagram.Application.Validators;
using FluentValidation;

namespace Fatagram.API.Extensions.Services
{
    public static class ServiceExtensions
    {
        public static void AddApplicationServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddHttpContextAccessor();

            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            // Infrastructure
            builder.Services.AddDatabaseServices(builder.Configuration);
            builder.Services.AddCacheServices(builder.Configuration);
            builder.Services.AddCloudStorageServices(builder.Configuration);
            builder.Services.AddDependencyServices();

            // Auth
            builder.Services.AddAuthenticationServices(builder.Configuration);

            // Presentation
            builder.AddCorsServices();
            builder.Services.AddSignalRServices();
            builder.Services.AddControllerServices();
            builder.Services.AddApiVersioningServices();
            builder.Services.AddSwaggerServices();

            // FluentValidation
            builder.Services.AddValidatorsFromAssemblyContaining<RegisterDtoValidator>();
        }
    }
}

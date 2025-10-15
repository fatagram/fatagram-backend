using System.Text.Json.Serialization;
using Fatagram.API.Extensions.Constrains;
using Fatagram.API.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Fatagram.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddServices(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            services.AddHttpContextAccessor();

            // Add services to the container.
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
            });

            // Turn off ModelStateInvalidFilter
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            // Add CORS
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

            services
                .AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(null));
                });

            // Add authentication
            services
                .AddAuthentication("JwtAuthenticationScheme")
                .AddScheme<AuthenticationSchemeOptions, JwtAuthenticationHandler>(
                    "JwtAuthenticationScheme",
                    null
                );

            // SignalR
            services.AddSignalR();

            // Add application services
            services.AddDependencies();
        }
    }
}

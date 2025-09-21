using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Fatagram.API.Extensions.ApiVersioning
{
    /// <summary>
    /// Extension methods for configuring API versioning (Custom Implementation)
    /// </summary>
    public static class ApiVersioningExtensions
    {
        /// <summary>
        /// Adds custom API versioning configuration to the service collection
        /// </summary>
        public static IServiceCollection AddCustomApiVersioning(this IServiceCollection services)
        {
            // Register version provider
            services.AddSingleton<IApiVersionProvider, ApiVersionProvider>();
            services.AddScoped<ApiVersionMiddleware>();
            
            return services;
        }

        /// <summary>
        /// Configures Swagger to support multiple API versions
        /// </summary>
        public static IServiceCollection AddVersionedSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                // Configure for multiple versions
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1.0",
                    Title = "Fatagram API V1",
                    Description = "Fatagram Social Media API - Version 1.0",
                    Contact = new OpenApiContact
                    {
                        Name = "Fatagram Development Team",
                        Email = "dev@fatagram.com"
                    }
                });

                options.SwaggerDoc("v2", new OpenApiInfo
                {
                    Version = "v2.0", 
                    Title = "Fatagram API V2",
                    Description = "Fatagram Social Media API - Version 2.0 (Enhanced Features)",
                    Contact = new OpenApiContact
                    {
                        Name = "Fatagram Development Team",
                        Email = "dev@fatagram.com"
                    }
                });

                // Include XML comments
                var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    options.IncludeXmlComments(xmlPath);
                }

                // Filter endpoints by version
                options.DocInclusionPredicate((version, desc) =>
                {
                    if (!desc.TryGetMethodInfo(out var methodInfo))
                        return false;

                    // Check controller for version attributes
                    var controllerType = methodInfo.DeclaringType;
                    var versions = GetVersionsFromController(controllerType);
                    
                    if (!versions.Any())
                        return version == "v1"; // Default to v1
                        
                    return versions.Contains(version);
                });

                // Add authorization header
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            return services;
        }

        /// <summary>
        /// Configures versioned Swagger UI
        /// </summary>
        public static IApplicationBuilder UseVersionedSwagger(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Fatagram API V1");
                options.SwaggerEndpoint("/swagger/v2/swagger.json", "Fatagram API V2");
                
                options.RoutePrefix = "swagger";
                options.DocumentTitle = "Fatagram API Documentation";
                options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
                options.DisplayRequestDuration();
            });

            return app;
        }

        /// <summary>
        /// Uses API versioning middleware
        /// </summary>
        public static IApplicationBuilder UseCustomApiVersioning(this IApplicationBuilder app)
        {
            app.UseMiddleware<ApiVersionMiddleware>();
            return app;
        }

        private static List<string> GetVersionsFromController(Type? controllerType)
        {
            if (controllerType == null) return new List<string>();

            var versions = new List<string>();
            
            // Check for custom version attributes
            var versionAttrs = controllerType.GetCustomAttributes(typeof(ApiVersionAttribute), true)
                .Cast<ApiVersionAttribute>();
                
            foreach (var attr in versionAttrs)
            {
                versions.AddRange(attr.Versions);
            }

            return versions;
        }
    }
}
using System.Xml;
using Asp.Versioning.ApiExplorer;
using Fatagram.API.Extensions;
using Fatagram.API.Extensions.Configuration;
using Fatagram.API.Extensions.Constrains;
using Fatagram.API.Extensions.Middleware;
using Fatagram.API.Hubs;
using Fatagram.API.Middlewares;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.SignalR;

namespace Fatagram.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.ConfigureAppConfiguration();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    var provider =
                        app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
                    foreach (var description in provider.ApiVersionDescriptions)
                    {
                        options.SwaggerEndpoint(
                            $"/swagger/{description.GroupName}/swagger.json",
                            description.GroupName.ToUpperInvariant()
                        );
                    }
                });
            }

            app.UseRouting();
            // Cors
            app.UseCors(CorsPolicySettings.MyAllowSpecificOrigins);

            // Static files
            app.UseStaticFiles();

            // Authentication
            app.UseAuthentication();
            app.UseAuthorization();

            // Middlewares
            app.UseCustomMiddlewares();

            // Routing
            app.MapControllers().RequireCors(CorsPolicySettings.MyAllowSpecificOrigins);
            app.MapGet("/hi", () => "Hello World!");
            app.MapHub<NotificationHub>("/hubs/notification");

            app.Run();
        }
    }
}

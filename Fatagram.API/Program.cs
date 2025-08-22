using Fatagram.API.Extensions.Constrains;
using Fatagram.API.Extensions.Dependencies;
using Fatagram.API.Extensions.ExceptionHandlerExtensions;
using Fatagram.API.Extensions.ServiceCollectionExtensions;
using Fatagram.API.Extensions.WebApplicationBuilderExtensions;
using Fatagram.API.Hubs;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.SignalR;
using System.Xml;

namespace Fatagram.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            
            builder.Configuration
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true);

            // Configure services
            builder.Services.AddServices(builder.Configuration);
            builder.ConfigureKestrelOptions();

            // Max request body size
            builder.WebHost.UseKestrel(option => {
                option.Limits.MaxRequestBodySize = 2 * 1024 * 1024; // 2MB
            });
            
            // Create app 
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseRouting();

            // Cors 
            app.UseCors(CorsPolicySettings.MyAllowSpecificOrigins);

            // Exception handling
            app.ConfigureExceptionHandler();

            // Redirect HTTP to HTTPS
            // if (!Setups.IsForLAN)
            //     app.UseHttpsRedirection();

            // Static files
            app.UseStaticFiles();

            // Authentication
            app.UseAuthentication();
            app.UseAuthorization();

            // Routing
            app.MapControllers().RequireCors(CorsPolicySettings.MyAllowSpecificOrigins);
            app.MapHub<NotificationHub>("/hubs/notification");

            // var hubContext = app.Services.GetRequiredService<IHubContext<NotificationHub>>();

            // Start the application
            app.Run();
        }
    }
}

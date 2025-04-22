using Fatagram.API.Extensions.Constrains;
using Fatagram.API.Extensions.Dependencies;
using Fatagram.API.Extensions.ExceptionHandlerExtensions;
using Fatagram.API.Extensions.ServiceCollectionExtensions;
using Fatagram.API.Extensions.WebApplicationBuilderExtensions;
using Microsoft.AspNetCore.Diagnostics;

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
            
            // Create app 
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Cors 
            app.UseCors(CorsPolicySettings.MyAllowSpecificOrigins);

            // Exception handling
            app.ConfigureExceptionHandler();
            
            // Redirect HTTP to HTTPS
            app.UseHttpsRedirection();

            // Static files
            app.UseStaticFiles();

            // Authentication
            app.UseAuthentication();
            app.UseAuthorization();

            // Routing
            app.MapControllers();

            // Start the application
            app.Run();
        }
    }
}

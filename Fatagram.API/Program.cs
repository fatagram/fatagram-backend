using Fatagram.API.Authentication;
using Fatagram.Application;
using Fatagram.Application.Services;
using Fatagram.Application.Services.Interfaces;
using Fatagram.Infrastructure.Data;
using Fatagram.Infrastructure.Repositories;
using Fatagram.Infrastructure.Repositories.Interfaces;
using Fatagram.Infrastructure.Repositories.MockDB;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fatagram.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });
            
            // Dependency injection

            // Scoped for services
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped<IAccountService, AccountService>();
            builder.Services.AddScoped<IJwtService, JwtHmacSha256Service>();
            builder.Services.AddScoped<IPrivacySettingsService, PrivacySettingsService>();
            builder.Services.AddScoped<IOtherUserService, OtherUserService>();

            builder.Services.AddAutoMapper(typeof(Mapping));

            // For MockDB
            if (builder.Configuration.GetValue<bool>("UseMockDB"))
            {
                // Use singleton for MockDB because we want to keep the data
                builder.Services.AddSingleton<IUserRepository, MUserRepository>();
                builder.Services.AddSingleton<IAccountRepository, MAccountRepository>();
                builder.Services.AddSingleton<IRefreshTokenRepository, MRefreshTokenRepository>();
                builder.Services.AddSingleton<IPrivarySettingRepository, MPrivacySettingRepository>();

            }
            else
            {
                builder.Services.AddScoped<IUserRepository, UserRepository>();
                builder.Services.AddScoped<IAccountRepository, AccountRepository>();
                builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            }


            // Singleton
            builder.Services.AddSingleton<JwtHmacSha256Service>();
            builder.Services.AddSingleton(TimeProvider.System);


            // Turn off ModelStateInvalidFilter
            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });


            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var _myAllowSpecificOrigins = "_myAllowSpecificOrigins";


            // Add CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: _myAllowSpecificOrigins,
                        builder =>
                        {
                            builder.SetIsOriginAllowed(origin => true)
                                .AllowAnyHeader()
                                .AllowAnyMethod()
                                .AllowCredentials();
                        });
            });

            // Add authentication
            builder.Services.AddAuthentication("JwtAuthenticationScheme")
                .AddScheme<AuthenticationSchemeOptions, JwtAuthenticationHandler>("JwtAuthenticationScheme", null);


            // Add authorization
            builder.WebHost.ConfigureKestrel(options =>
            {
                options.ListenAnyIP(5000);
                //options.ListenAnyIP(5001, listenOptions =>
                //{
                //    listenOptions.UseHttps();
                //});
            });



            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }


            app.UseCors(_myAllowSpecificOrigins);
            app.UseHttpsRedirection();

            // Check access token


            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}

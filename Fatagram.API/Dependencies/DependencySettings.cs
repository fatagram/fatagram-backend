using Fatagram.Application.Services.ImageService.Interface;
using Fatagram.Application.Services.ImageService;
using Fatagram.Application.Common;

namespace Fatagram.API.Dependencies
{
    public static class DependencySettings
    {
        public static void AddDependencies(this WebApplicationBuilder builder)
        {
            // Scoped for services
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IUserPrivacyService, UserPrivacyService>();
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped<IAccountService, AccountService>();
            builder.Services.AddScoped<IJwtService, JwtHmacSha256Service>();
            builder.Services.AddScoped<IImageService, WwwrootImageService>();

            builder.Services.AddScoped<IUserPrivacyRepository, UserPrivacyRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IAccountRepository, AccountRepository>();
            builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

            builder.Services.AddAutoMapper(typeof(Mapping));

            // Singleton
            builder.Services.AddSingleton<JwtHmacSha256Service>();
            builder.Services.AddSingleton(TimeProvider.System);
        }
    }
}

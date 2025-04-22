using Fatagram.Application.Services.ImageService.Interface;
using Fatagram.Application.Services.ImageService;
using Fatagram.Application.Common;
using Fatagram.Application.Checker;

namespace Fatagram.API.Extensions.Dependencies
{
    public static class DependencyExtensions
    {
        public static void AddDependencies(this IServiceCollection services)
        {
            // Scoped for services
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserPrivacyService, UserPrivacyService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<UserChecker>();

            services.AddScoped<IJwtService, JwtHmacSha256Service>();
            services.AddScoped<IImageService, WwwrootImageService>();

            services.AddScoped<IUserPrivacyRepository, UserPrivacyRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

            services.AddAutoMapper(typeof(Mapping));

            // Singleton
            services.AddSingleton<JwtHmacSha256Service>();
            services.AddSingleton(TimeProvider.System);
        }
    }
}

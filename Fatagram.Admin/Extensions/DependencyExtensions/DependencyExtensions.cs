using Fatagram.Application.Services.ImageService.Interface;
using Fatagram.Application.Services.ImageService;
using Fatagram.Application.Common;
using Fatagram.Application.Checker;
using Fatagram.Infrastructure.Repositories.FriendshipRepository.Interfaces;
using Fatagram.Infrastructure.Repositories.FriendshipRepository;
using Fatagram.Infrastructure.Repositories.FriendRequestRepository;
using Fatagram.Infrastructure.Repositories.FriendRequestRepository.Interfaces;
using Fatagram.Application.Services.AuthServices.Interface;
using Fatagram.Application.Services.AuthService;
using Fatagram.Application.Services.UserServices.Interface;
using Fatagram.Infrastructure.Repositories.UserPrivacyRepository.Interface;
using Fatagram.Application.Services.TokenServices.Interface;
using Fatagram.Application.Services.TokenServices;
using Fatagram.Application.Services.AccountServices.Interface;
using Fatagram.Application.Services.AccountServices;
using Fatagram.Application.Services.JwtServices.Interface;
using Fatagram.Application.Services.JwtServices;
using Fatagram.Infrastructure.Repositories.UserPrivacyRepository;
using Fatagram.Infrastructure.Repositories.UserRepository.Interface;
using Fatagram.Infrastructure.Repositories.UserRepository;
using Fatagram.Infrastructure.Repositories.AccountRepository.Interface;
using Fatagram.Infrastructure.Repositories.AccountRepository;
using Fatagram.Infrastructure.Repositories.RefreshTokenRepository.Interface;
using Fatagram.Infrastructure.Repositories.RefreshTokenRepository;
using Fatagram.Application.Services.UserServices;

namespace Fatagram.Admin.Extensions.Dependencies
{
    public static class DependencyExtensions
    {
        public static void AddDependencies(this IServiceCollection services)
        {
            // Scoped for services
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserPrivacyRepository, UserPrivacyRepository>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<UserChecker>();

            services.AddScoped<IJwtService, JwtHmacSha256Service>();
            services.AddScoped<IImageService, NginxImageService>();

            services.AddScoped<IUserPrivacyRepository, UserPrivacyRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IFriendshipRepository, FriendshipRepository>();
            services.AddScoped<IFriendRequestRepository, FriendRequestRepository>();

            services.AddAutoMapper(typeof(Mapping));

            // Singleton
            services.AddSingleton<JwtHmacSha256Service>();
            services.AddSingleton(TimeProvider.System);
        }
    }
}

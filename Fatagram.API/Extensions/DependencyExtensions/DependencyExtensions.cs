using Fatagram.Application.Services.ImageService.Interface;
using Fatagram.Application.Services.ImageService;
using Fatagram.Application.Common;
using Fatagram.Infrastructure.Repositories.FriendshipRepository.Interfaces;
using Fatagram.Infrastructure.Repositories.FriendshipRepository;
using Fatagram.Infrastructure.Repositories.FriendRequestRepository;
using Fatagram.Infrastructure.Repositories.FriendRequestRepository.Interfaces;
using AutoMapper;
using Fatagram.API.Hubs;
using Microsoft.AspNetCore.SignalR;
using Fatagram.Application.Services.NotificationServices.Interfaces;
using Fatagram.API.Hubs.Notifications;
using Fatagram.Application.Services.NotificationServices.Interface;
using Fatagram.Infrastructure.Repositories.NotificationRepository.Interface;
using Fatagram.Infrastructure.Repositories.NotificationRepository;
using Fatagram.Application.Services.NotificationServices;

namespace Fatagram.API.Extensions.Dependencies
{
    public static class DependencyExtensions
    {
        public static void AddDependencies(this IServiceCollection services)
        {
            // Scoped for services
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserProfileService, UserProfileService>();
            services.AddScoped<IFriendshipService, FriendshipService>();
            services.AddScoped<IUserPrivacyService, UserPrivacyService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IJwtService, JwtHmacSha256Service>();
            services.AddScoped<IImageService, NginxImageService>();

            services.AddScoped<IUserPrivacyRepository, UserPrivacyRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IFriendshipRepository, FriendshipRepository>();
            services.AddScoped<IFriendRequestRepository, FriendRequestRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();

            services.AddScoped<INotificationSender, NotificationSender>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<NotificationInfoService>();

            services.AddAutoMapper(typeof(Mapping));

            // Singleton
            services.AddSingleton<JwtHmacSha256Service>();
            services.AddSingleton(TimeProvider.System);
            // services.AddSingleton<IUserIdProvider, UserIdProvider>();
        }
    }
}

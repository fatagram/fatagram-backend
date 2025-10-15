using AutoMapper;
using Fatagram.API.Hubs;
using Fatagram.API.Hubs.Notifications;
using Fatagram.Application.Common;
using Fatagram.Application.Services.ImageService;
using Fatagram.Application.Services.ImageService.Interface;
using Fatagram.Application.Services.NotificationServices;
using Fatagram.Application.Services.NotificationServices.Interface;
using Fatagram.Application.Services.NotificationServices.Interfaces;
using Fatagram.Application.Services.UserServices.UserConfigServices;
using Fatagram.Application.Services.UserServices.UserConfigServices.Interfaces;
using Fatagram.Infrastructure.Repositories.FriendRequestRepository;
using Fatagram.Infrastructure.Repositories.FriendRequestRepository.Interfaces;
using Fatagram.Infrastructure.Repositories.FriendshipRepository;
using Fatagram.Infrastructure.Repositories.FriendshipRepository.Interfaces;
using Fatagram.Infrastructure.Repositories.NotificationRepository;
using Fatagram.Infrastructure.Repositories.NotificationRepository.Interface;
using Microsoft.AspNetCore.SignalR;

namespace Fatagram.API.Extensions
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
            services.AddScoped<IImageService, LocalImageService>();
            services.AddScoped<IUserConfigService, UserConfigService>();
            services.AddScoped<IUserInfoService, UserInfoService>();

            // Scoped for repositories
            services.AddScoped<IUserPrivacyRepository, UserPrivacyRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IFriendshipRepository, FriendshipRepository>();
            services.AddScoped<IFriendRequestRepository, FriendRequestRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<INotificationContentRepository, NotificationContentRepository>();
            services.AddScoped<IUserInformationRepository, UserInformationRepository>();

            // Scoped for SignalR
            services.AddScoped<INotificationSender, NotificationSender>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<NotificationInfoService>();

            // Scoped for AutoMapper
            services.AddAutoMapper(typeof(Mapping));

            // Singleton
            services.AddSingleton<JwtHmacSha256Service>();
            services.AddSingleton(TimeProvider.System);
            // services.AddSingleton<IUserIdProvider, UserIdProvider>();
        }
    }
}

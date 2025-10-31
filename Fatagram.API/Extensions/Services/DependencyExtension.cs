using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fatagram.API.Extensions.Services
{
    public static class DependencyExtension
    {
        public static void AddDependencyServices(this IServiceCollection services)
        {
            // Scoped for services
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserProfileService, UserProfileService>();
            services.AddScoped<IFriendshipService, FriendshipService>();
            services.AddScoped<IUserPrivacyService, UserPrivacyService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IImageService, LocalImageService>();
            services.AddScoped<IUserConfigService, UserConfigService>();
            services.AddScoped<IUserInfoService, UserInfoService>();

            // Scoped for repositories
            services.AddScoped<IUserPrivacyRepository, UserPrivacyRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IAccountRepository, AccountRepository>();
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
            services.AddSingleton(TimeProvider.System);
        }
    }
}

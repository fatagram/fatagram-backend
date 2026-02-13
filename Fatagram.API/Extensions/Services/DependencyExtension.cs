using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.Application.Common.Mapper;
using Fatagram.Domain.Models;
using Fatagram.Infrastructure.Repositories.BaseRepository;
using Fatagram.Infrastructure.Repositories.BaseRepository.Interfaces;
using Fatagram.Infrastructure.Repositories.EmailRepository;
using Fatagram.Infrastructure.Repositories.EmailRepository.Interfaces;
using Fatagram.Infrastructure.Repositories.UserEmailRepository;
using Fatagram.Infrastructure.Repositories.UserEmailRepository.Interfaces;

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
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IJwtService, JwtHmacSha256Service>();
            services.AddScoped<IImageService, LocalImageService>();
            services.AddScoped<IUserConfigService, UserConfigService>();
            services.AddScoped<OAuthServiceFactory>();

            // Friendship service dependencies
            services.AddScoped<IFriendRequestManager, FriendRequestManager>();
            services.AddScoped<FriendshipValidator>();
            services.AddScoped<IFriendshipNotificationStrategy, FriendshipNotificationStrategy>();

            // Scoped for repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<IEmailRepository, EmailRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IFriendshipRepository, FriendshipRepository>();
            services.AddScoped<IFriendRequestRepository, FriendRequestRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<INotificationContentRepository, NotificationContentRepository>();
            services.AddScoped<IUserEmailRepository, UserEmailRepository>();

            // Scoped for SignalR
            services.AddScoped<INotificationSender, NotificationSender>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<NotificationInfoService>();

            // Scoped for AutoMapper
            services.AddAutoMapper(typeof(Mapping));

            // Singleton
            services.AddSingleton<JwtHmacSha256Service>();
            services.AddSingleton(TimeProvider.System);
        }
    }
}

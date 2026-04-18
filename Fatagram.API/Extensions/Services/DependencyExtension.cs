using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Fatagram.API.Hubs;
using Fatagram.API.Scripts;
using Fatagram.Application.Common.Mapper;
using Fatagram.Application.Services.ConversationServices;
using Fatagram.Application.Services.ConversationServices.Interfaces;
using Fatagram.Application.Services.MediaServices;
using Fatagram.Application.Services.MediaServices.Interfaces;
using Fatagram.Application.Services.MessageServices;
using Fatagram.Application.Services.MessageServices.Interfaces;
using Fatagram.Application.Services.SockerServices.Interfaces;
using Fatagram.Application.Services.UserServices.UserServices;
using Fatagram.Application.Services.UserServices.UserServices.Interfaces;
using Fatagram.Application.Utils;
using Fatagram.Infrastructure.Repositories.ConversationParticipantRepository;
using Fatagram.Infrastructure.Repositories.ConversationParticipantRepository.Interfaces;
using Fatagram.Infrastructure.Repositories.ConversationRepository;
using Fatagram.Infrastructure.Repositories.ConversationRepository.Interfaces;
using Fatagram.Infrastructure.Repositories.EmailRepository;
using Fatagram.Infrastructure.Repositories.EmailRepository.Interfaces;
using Fatagram.Infrastructure.Repositories.MediaRepository;
using Fatagram.Infrastructure.Repositories.MediaRepository.Interfaces;
using Fatagram.Infrastructure.Repositories.MessageRepository;
using Fatagram.Infrastructure.Repositories.MessageRepository.Interfaces;
using Fatagram.Infrastructure.Repositories.UserEmailRepository;
using Fatagram.Infrastructure.Repositories.UserEmailRepository.Interfaces;
using Scrutor;

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
            services.AddScoped<IImageService, CloudImageService>();
            services.AddScoped<IUserConfigService, UserConfigService>();
            services.AddScoped<OAuthServiceFactory>();
            services.AddScoped<IConversationService, ConversationService>();
            services.AddScoped<IMessageService, MessageService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IConversationParticipantService, ConversationParticipantService>();
            services.AddScoped<IMediaService, MediaService>();

            // Friendship service dependencies
            services.AddScoped<IFriendRequestManager, FriendRequestManager>();
            services.AddScoped<FriendshipValidator>();

            // Scoped for repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<IEmailRepository, EmailRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IFriendshipRepository, FriendshipRepository>();
            services.AddScoped<IFriendRequestRepository, FriendRequestRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<IUserEmailRepository, UserEmailRepository>();
            services.AddScoped<IUserNotificationRepository, UserNotificationRepository>();
            services.AddScoped<IConversationRepository, ConversationRepository>();
            services.AddScoped<
                IConversationParticipantRepository,
                ConversationParticipantRepository
            >();
            services.AddScoped<IMessageRepository, MessageRepository>();
            services.AddScoped<IMediaRepository, MediaRepository>();

            // Scoped for cached repositories
            services.Decorate<IConversationRepository, CachedConversationRepository>();
            services.Decorate<IMessageRepository, CachedMessageRepository>();
            services.Decorate<
                IConversationParticipantRepository,
                CachedConvParticipantRepository
            >();
            services.Decorate<IMediaRepository, CachedMediaRepository>();

            // Scoped for SignalR
            services.AddScoped(typeof(ISocketSender<>), typeof(SocketSender<>));
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<NotificationInfoService>();

            // Transient: each injection gets fresh builder (no shared state)
            services.AddTransient<NotifyBuilder>();

            // Scoped for AutoMapper
            services.AddAutoMapper(typeof(Mapping));

            // Singleton
            services.AddSingleton<JwtHmacSha256Service>();
            services.AddSingleton(TimeProvider.System);

            var commandType = typeof(ICommand);
            var scriptCommandTypes = Assembly
                .GetExecutingAssembly()
                .GetTypes()
                .Where(type =>
                    commandType.IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract
                );

            foreach (var scriptCommandType in scriptCommandTypes)
            {
                services.AddScoped(commandType, scriptCommandType);
            }
        }
    }
}

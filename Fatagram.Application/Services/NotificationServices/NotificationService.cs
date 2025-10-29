using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Fatagram.Application.Dtos.Notification;
using Fatagram.Application.Dtos.Query;
using Fatagram.Application.Services.NotificationServices.Interfaces;
using Fatagram.Application.Utils;
using Fatagram.Domain.Enums.NotificationServices;
using Fatagram.Domain.Models;
using Fatagram.Infrastructure.Repositories.NotificationRepository.Interface;
using Fatagram.Infrastructure.Repositories.UserRepository.Interface;
using Fatagram.Shared.Extensions;

namespace Fatagram.Application.Services.NotificationServices.Interface
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly INotificationContentRepository _notificationContentRepository;
        private readonly INotificationSender _notificationSender;
        private readonly IUserRepository _userRepository;
        private readonly NotificationInfoService _notificationInfoService;
        private readonly IMapper _mapper;

        public NotificationService(
            INotificationRepository notificationRepository,
            INotificationContentRepository notificationContentRepository,
            NotificationInfoService notificationInfoService,
            IUserRepository userRepository,
            IMapper mapper,
            INotificationSender notificationSender
        )
        {
            _mapper = mapper;
            _notificationRepository = notificationRepository;
            _notificationContentRepository = notificationContentRepository;
            _notificationSender = notificationSender;
            _notificationInfoService = notificationInfoService;
            _userRepository = userRepository;
        }

        public async Task CreateNotificationAsync(
            Guid userId,
            NotificationDto notificationDto,
            bool isSave = true
        )
        {
            var attachedNotification =
                await _notificationInfoService.AttachInfosToNotificationAsync(notificationDto);
            if (isSave)
            {
                var notification = _mapper.Map<Notification>(attachedNotification);
                await _notificationRepository.AddAsync(notification);
                attachedNotification.Id = notification.Id.ToString();
            }
            // Send notification to the user here
            var userLang = await _userRepository.GetLanguageAsync(
                attachedNotification.UserId.ToGuid()
            );
            var notificationContent = await _notificationContentRepository.GetContentAsync(
                attachedNotification.Type,
                userLang
            );
            attachedNotification.Content = notificationContent;
            await _notificationSender.SendNotificationAsync(userId, attachedNotification);
        }

        public async Task DeleteAllNotificationsAsync(Guid userId)
        {
            await _notificationRepository.DeleteAllNotificationsAsync(userId);
        }

        public async Task DeleteNotificationAsync(Guid notificationId)
        {
            await _notificationRepository.DeleteNotificationAsync(notificationId);

            // Send cancel notification to the user
            // var cancelNotification = NotificationFactory.CreateCancelNotification()
        }

        public async Task<CursorPagedResult<Guid?, NotificationDto>> GetNotificationsAsync(
            Guid userId,
            CursorQuery<Guid?> query
        )
        {
            var language = await _userRepository.GetLanguageAsync(userId);
            var data = await _notificationRepository.GetNotificationsAsync(
                userId,
                language,
                query.Cursor,
                query.Limit
            );

            var result = data
                .notifications.Select(async n =>
                    await _notificationInfoService.AttachInfosToNotificationAsync(
                        _mapper.Map<NotificationDto>(n)
                    )
                )
                .Select(t => t.Result)
                .ToList();

            return CursorPagedResult<Guid?, NotificationDto>.Success(
                result,
                query.Cursor,
                "Get notifications success",
                new Dictionary<string, object> { { "UnreadCount", data.unreadCount } }
            );
        }

        public async Task<CursorPagedResult<Guid?, NotificationDto>> GetUnreadNotificationsAsync(
            Guid userId,
            CursorQuery<Guid?> query
        )
        {
            var language = await _userRepository.GetLanguageAsync(userId);
            var data = await _notificationRepository.GetUnreadNotificationsAsync(
                userId,
                language,
                query.Cursor,
                query.Limit
            );

            var notificationsWithInfo = await Task.WhenAll(
                data.notifications.Select(async n =>
                    await _notificationInfoService.AttachInfosToNotificationAsync(
                        _mapper.Map<NotificationDto>(n)
                    )
                )
            );

            return CursorPagedResult<Guid?, NotificationDto>.Success(
                notificationsWithInfo,
                query.Cursor,
                "Get unread notifications success",
                new Dictionary<string, object> { { "UnreadCount", data.unreadCount } }
            );
        }

        public async Task MarkNotificationAsReadAsync(Guid notificationId)
        {
            await _notificationRepository.MarkNotificationAsReadAsync(notificationId);
        }

        public async Task MarkAllNotificationsAsReadAsync(Guid userId)
        {
            await _notificationRepository.MarkAllNotificationsAsReadAsync(userId);
        }

        private async Task<IEnumerable<Notification>> FindNotificationAsync(
            Guid userId,
            Guid actorId,
            NotificationType type,
            Dictionary<string, string>? data
        )
        {
            return await _notificationRepository.FindNotifications(
                userId,
                actorId,
                type,
                data ?? new()
            );
        }

        public async Task DeleteNotificationsAsync(
            Guid userId,
            Guid actorId,
            NotificationType type,
            Dictionary<string, string>? data = null,
            bool isSendCancel = true
        )
        {
            var notifications = await FindNotificationAsync(userId, actorId, type, data);
            foreach (var notification in notifications)
            {
                await _notificationRepository.DeleteNotificationAsync(notification.Id);
                if (isSendCancel)
                {
                    var cancelNotification = NotificationFactory.CreateCancelNotification(
                        userId,
                        notification.Id
                    );
                    await _notificationSender.SendNotificationAsync(userId, cancelNotification);
                }
            }
        }
    }
}

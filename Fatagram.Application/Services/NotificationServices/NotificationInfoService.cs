using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.Application.Dtos.Notification;
using Fatagram.Application.Services.NotificationServices.AttachInfos;
using Fatagram.Application.Services.NotificationServices.Interface;
using Fatagram.Application.Services.NotificationServices.Interfaces;
using Fatagram.Domain.Enums.NotificationServices;
using Fatagram.Domain.Models;
using Fatagram.Infrastructure.Repositories.UserRepository.Interface;
using Fatagram.Shared.Extensions;

namespace Fatagram.Application.Services.NotificationServices
{
    public class NotificationInfoService
    {
        private readonly IUserRepository _userRepository;

        public NotificationInfoService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<NotificationDto> AttachInfosToNotificationAsync(
            NotificationDto notification
        )
        {
            var attachInfoService = GetAttachInfoService(notification.Type);
            if (attachInfoService != null)
            {
                notification = await attachInfoService.AttachAsync(notification);
            }
            return notification;
        }

        public async Task<List<NotificationDto>?> AttachInfosToNotificationsAsync(
            List<NotificationDto> notifications
        )
        {
            if (notifications == null || notifications.Count == 0)
                return notifications;

            var notificationsWithIndex = notifications
                .Select((n, index) => new { Notification = n, Index = index })
                .ToList();

            var groupedByType = notificationsWithIndex.GroupBy(x => x.Notification.Type);

            var resultDict = new Dictionary<int, NotificationDto>();

            foreach (var group in groupedByType)
            {
                var attachInfoService = GetAttachInfoService(group.Key);

                if (attachInfoService != null)
                {
                    var notificationsInGroup = group.Select(x => x.Notification).ToList();
                    var attachedNotifications = await attachInfoService.AttachAsync(
                        notificationsInGroup
                    );

                    var groupIndexes = group.Select(x => x.Index).ToList();
                    for (int i = 0; i < attachedNotifications.Count; i++)
                    {
                        resultDict[groupIndexes[i]] = attachedNotifications[i];
                    }
                }
                else
                {
                    foreach (var item in group)
                    {
                        resultDict[item.Index] = item.Notification;
                    }
                }
            }

            return notificationsWithIndex.Select(x => resultDict[x.Index]).ToList();
        }

        private INotificationAttachInfo? GetAttachInfoService(NotificationType notificationType)
        {
            return notificationType switch
            {
                NotificationType.NewFriendRequest => new NewFriendRequestAttachInfo(
                    _userRepository
                ),
                _ => null,
            };
        }
    }
}

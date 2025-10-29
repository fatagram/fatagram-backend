using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.Application.Dtos.Notification;
using Fatagram.Domain.Enums.NotificationServices;
using Fatagram.Domain.Models;
using Fatagram.Shared.Enums;
using Fatagram.Shared.Extensions;

namespace Fatagram.Application.Services.NotificationServices
{
    public static class NotificationFactory
    {
        public static NotificationDto CreateNewFriendRequestNotification(Guid userId, Guid senderId)
        {
            return new NotificationDto
            {
                UserId = userId.ToString(),
                ActorId = senderId.ToString(),
                Link = $"/{senderId}",
                Type = NotificationType.NewFriendRequest,
                IsRead = false,
                TimeDistance = new TimeDistance(0, TimeUnit.Miliseconds),
            };
        }

        public static NotificationDto CreateFriendRequestAcceptedNotification(
            Guid userId,
            Guid senderId
        )
        {
            return new NotificationDto
            {
                UserId = userId.ToString(),
                ActorId = senderId.ToString(),
                Link = $"{userId}",
                Type = NotificationType.FriendRequestAccepted,
                IsRead = false,
                TimeDistance = new TimeDistance(0, TimeUnit.Miliseconds),
            };
        }

        public static NotificationDto CreateSystemNotification(
            string userId,
            string message,
            string link
        )
        {
            return new NotificationDto
            {
                UserId = userId,
                Data = new Dictionary<string, string> { { "message", message } },
                Link = link,
                Type = NotificationType.System,
                IsRead = false,
                TimeDistance = new TimeDistance(0, TimeUnit.Miliseconds),
            };
        }

        public static NotificationDto CancelFriendRequestNotification(
            string userId,
            string notificationId
        )
        {
            return new NotificationDto
            {
                UserId = userId,
                Type = NotificationType.FriendRequestCanceled,
                Data = new Dictionary<string, string> { { "noticationId", notificationId } },
                IsRead = false,
                TimeDistance = new TimeDistance(0, TimeUnit.Miliseconds),
            };
        }

        public static NotificationDto CreateCancelNotification(Guid userId, Guid notificationId)
        {
            return new NotificationDto
            {
                UserId = userId.ToString(),
                Type = NotificationType.CancelNotification,
                Data = new Dictionary<string, string>
                {
                    { "noticationId", notificationId.ToString() },
                },
                IsRead = false,
                TimeDistance = new TimeDistance(0, TimeUnit.Miliseconds),
            };
        }
    }
}

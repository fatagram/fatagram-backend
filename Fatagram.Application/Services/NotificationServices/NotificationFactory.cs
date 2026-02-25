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
        public static NotificationDto CreateNewFriendRequestNotification(
            Guid userId,
            Guid senderId,
            Guid sourceId
        )
        {
            return new NotificationDto
            {
                UserId = userId.ToString(),
                ActorId = senderId.ToString(),
                SourceId = sourceId.ToString(),
                Link = $"/{senderId}",
                Type = NotificationType.NewFriendRequest,
                IsRead = false,
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
                Data = new Dictionary<string, string> { { "notificationId", notificationId } },
                IsRead = false,
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
                    { "notificationId", notificationId.ToString() },
                },
                IsRead = false,
            };
        }
    }
}

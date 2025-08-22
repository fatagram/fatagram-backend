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
            string userId,
            string senderId,
            string link)
        {
            return new NotificationDto
            {
                UserId = userId,
                ActorId = senderId,
                Link = link,
                Type = NotificationType.NewFriendRequest,
                IsRead = false,
                TimeDistance = new TimeDistance(0, TimeUnit.Miliseconds)
            };
        }

        public static NotificationDto CreateFriendRequestAcceptedNotification(
            string userId,
            string senderId,
            string link)
        {
            return new NotificationDto
            {
                UserId = userId,
                ActorId = senderId,
                Link = link,
                Type = NotificationType.FriendRequestAccepted,
                IsRead = false,
                TimeDistance = new TimeDistance(0, TimeUnit.Miliseconds)
            };
        }

        public static NotificationDto CreateSystemNotification(
            string userId,
            string message,
            string link)
        {
            return new NotificationDto
            {
                UserId = userId,
                Data = new Dictionary<string, string>
                {
                    { "message", message },
                },
                Link = link,
                Type = NotificationType.System,
                IsRead = false,
                TimeDistance = new TimeDistance(0, TimeUnit.Miliseconds)
            };
        }

        public static NotificationDto CancelFriendRequestNotification(
            string userId,
            string notificationId)
        {
            return new NotificationDto
            {
                UserId = userId,
                Type = NotificationType.FriendRequestCanceled,
                Data = new Dictionary<string, string>
                {
                    { "noticationId", notificationId }
                },
                IsRead = false,
                TimeDistance = new TimeDistance(0, TimeUnit.Miliseconds)
            };
        }

        public static NotificationDto CreateCancelNotification(
            string userId,
            string notificationId
        )
        {
            return new NotificationDto
            {
                UserId = userId,
                Type = NotificationType.CancelNotification,
                Data = new Dictionary<string, string>
                {
                    { "noticationId", notificationId }
                },
                IsRead = false,
                TimeDistance = new TimeDistance(0, TimeUnit.Miliseconds)
            };
        }
    }
}
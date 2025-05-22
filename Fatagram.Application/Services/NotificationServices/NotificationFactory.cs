using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.Application.Dtos.Notification;
using Fatagram.Shared.Enums;
using Fatagram.Shared.Extensions;

namespace Fatagram.Application.Services.NotificationServices
{
    public static class NotificationFactory
    {
        public static NotificationDto CreateNewFriendRequestNotification(
            string userId,
            string senderName,
            string link)
        {
            return new NotificationDto
            {
                UserId = userId,
                Data = new Dictionary<string, string>
                {
                    { "senderName", senderName },
                    { "link", link }
                },
                IsRead = false,
                TimeDistance = new TimeDistance(0, TimeUnit.Miliseconds)
            };
        }

        public static NotificationDto CreateFriendRequestAcceptedNotification(
            string userId,
            string senderName,
            string link)
        {
            return new NotificationDto
            {
                UserId = userId,
                Data = new Dictionary<string, string>
                {
                    { "senderName", senderName },
                    { "link", link }
                },
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
                    { "link", link }
                },
                IsRead = false,
                TimeDistance = new TimeDistance(0, TimeUnit.Miliseconds)
            };
        }
    }
}
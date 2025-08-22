using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.Application.Dtos.Notification;
using Fatagram.Application.Utils;
using Fatagram.Domain.Enums.NotificationServices;
using Fatagram.Domain.Models;

namespace Fatagram.Application.Services.NotificationServices.Interface
{
    public interface INotificationService
    {
        Task CreateNotificationAsync(NotificationDto notificationDto, bool isSave = true);
        Task<Result<NotificationsDto>> GetNotificationsAsync(string userId, int page, int pageSize);
        Task<Result<NotificationsDto>> GetUnreadNotificationsAsync(string userId, int page, int pageSize);
        Task MarkNotificationAsReadAsync(string notificationId);
        Task MarkAllNotificationsAsReadAsync(string userId);
        Task DeleteNotificationAsync(string notificationId);
        Task DeleteAllNotificationsAsync(string userId);
        Task DeleteNotificationsAsync(
            string userId,
            string actorId,
            NotificationType type,
            Dictionary<string,
            string>? data = null,
            bool isSendCancel = true
        );
        // Task<IEnumerable<Notification>> FindNotificationAsync(string userId, NotificationType type, Dictionary<string, string> data);
    }
}
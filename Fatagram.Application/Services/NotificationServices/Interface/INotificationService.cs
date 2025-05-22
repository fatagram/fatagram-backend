using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.Application.Dtos.Notification;

namespace Fatagram.Application.Services.NotificationServices.Interface
{
    public interface INotificationService
    {
        Task CreateNotificationAsync(NotificationDto notificationDto);
        Task<IEnumerable<NotificationDto>> GetNotificationsAsync(string userId, int page, int pageSize);
        Task<IEnumerable<NotificationDto>> GetUnreadNotificationsAsync(string userId, int page, int pageSize);
        Task MarkNotificationAsReadAsync(string notificationId);
        Task MarkAllNotificationsAsReadAsync(string userId);
        Task DeleteNotificationAsync(string notificationId);
        Task DeleteAllNotificationsAsync(string userId);
    }
}
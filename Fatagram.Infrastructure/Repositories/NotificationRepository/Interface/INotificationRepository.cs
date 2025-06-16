using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.Application.Services.NotificationServices;
using Fatagram.Domain.Models;

namespace Fatagram.Infrastructure.Repositories.NotificationRepository.Interface
{
    public interface INotificationRepository
    {
        Task AddAsync(Notification notification);
        Task<(IEnumerable<Notification> notifications, int unreadCount)> GetNotificationsAsync(Guid userId, int page, int pageSize);
        Task<(IEnumerable<Notification> notifications, int unreadCount)> GetUnreadNotificationsAsync(Guid userId, int page, int pageSize);
        Task MarkNotificationAsReadAsync(Guid notificationId);
        Task MarkAllNotificationsAsReadAsync(Guid userId);
        Task DeleteNotificationAsync(Guid notificationId);
        Task DeleteAllNotificationsAsync(Guid userId);
        Task<IEnumerable<Notification>> FindNotifications(Guid userId, Guid actorId, NotificationType type, Dictionary<string, string> data);
    }
}
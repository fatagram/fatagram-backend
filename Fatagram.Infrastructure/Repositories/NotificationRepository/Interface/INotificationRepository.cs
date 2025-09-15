using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.Domain.Enums.NotificationServices;
using Fatagram.Domain.Models;
using Fatagram.Infrastructure.Projections;

namespace Fatagram.Infrastructure.Repositories.NotificationRepository.Interface
{
    public interface INotificationRepository
    {
        Task AddAsync(Notification notification);
        Task<(IEnumerable<NotificationProjection> notifications, int unreadCount)> GetNotificationsAsync(
            Guid userId, string langCode, Guid? cursorId, int pageSize);
        Task<(IEnumerable<NotificationProjection> notifications, int unreadCount)> GetUnreadNotificationsAsync(
            Guid userId, string langCode, Guid? cursorId, int pageSize);
        Task MarkNotificationAsReadAsync(Guid notificationId);
        Task MarkAllNotificationsAsReadAsync(Guid userId);
        Task DeleteNotificationAsync(Guid notificationId);
        Task DeleteAllNotificationsAsync(Guid userId);
        Task<IEnumerable<Notification>> FindNotifications(
            Guid userId, Guid actorId, NotificationType type, Dictionary<string, string> data);
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.Domain.Enums.NotificationServices;
using Fatagram.Domain.Models;
using Fatagram.Infrastructure.Projections;
using Fatagram.Infrastructure.Utils.Query;

namespace Fatagram.Infrastructure.Repositories.NotificationRepository.Interface
{
    public interface INotificationRepository
    {
        Task AddAsync(Notification notification);
        Task<(IEnumerable<NotificationProjection> notifications, int total)> GetNotificationsAsync(
            Guid userId,
            string langCode,
            CursorQuery<Guid> query
        );
        Task<(
            IEnumerable<NotificationProjection> notifications,
            int total
        )> GetUnreadNotificationsAsync(Guid userId, string langCode, CursorQuery<Guid> query);

        // Task<int> GetUnreadCountAsync(Guid userId);
        Task MarkNotificationAsReadAsync(Guid notificationId);
        Task MarkAllNotificationsAsReadAsync(Guid userId);
        Task DeleteNotificationAsync(Guid notificationId);
        Task DeleteAllNotificationsAsync(Guid userId);
        Task<IEnumerable<Notification>> FindNotifications(
            Guid userId,
            Guid actorId,
            NotificationType type,
            Dictionary<string, string> data
        );
    }
}

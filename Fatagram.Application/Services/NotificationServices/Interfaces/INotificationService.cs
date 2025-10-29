using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.Application.Dtos;
using Fatagram.Application.Dtos.Notification;
using Fatagram.Application.Dtos.Query;
using Fatagram.Application.Utils;
using Fatagram.Domain.Enums.NotificationServices;
using Fatagram.Domain.Models;

namespace Fatagram.Application.Services.NotificationServices.Interface
{
    public interface INotificationService
    {
        Task CreateNotificationAsync(
            Guid userId,
            NotificationDto notificationDto,
            bool isSave = true
        );
        Task<CursorPagedResult<Guid?, NotificationDto>> GetNotificationsAsync(
            Guid userId,
            CursorQuery<Guid?> query
        );
        Task<CursorPagedResult<Guid?, NotificationDto>> GetUnreadNotificationsAsync(
            Guid userId,
            CursorQuery<Guid?> query
        );
        Task MarkNotificationAsReadAsync(Guid notificationId);
        Task MarkAllNotificationsAsReadAsync(Guid userId);
        Task DeleteNotificationAsync(Guid notificationId);
        Task DeleteAllNotificationsAsync(Guid userId);
        Task DeleteNotificationsAsync(
            Guid userId,
            Guid actorId,
            NotificationType type,
            Dictionary<string, string>? data = null,
            bool isSendCancel = true
        );
        // Task<IEnumerable<Notification>> FindNotificationAsync(string userId, NotificationType type, Dictionary<string, string> data);
    }
}

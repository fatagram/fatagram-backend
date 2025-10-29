using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.Application.Dtos;
using Fatagram.Application.Dtos.Filter;
using Fatagram.Application.Dtos.Notification;
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
        Task<CursorResult<Guid?, NotificationDto>> GetNotificationsAsync(
            Guid userId,
            CursorFilter<Guid> query
        );
        Task<CursorResult<Guid?, NotificationDto>> GetUnreadNotificationsAsync(
            Guid userId,
            CursorFilter<Guid> query
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

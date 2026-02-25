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
        Task<CursorResult<DateTime, NotificationDto>> GetNotificationsAsync(
            Guid userId,
            CursorFilter<DateTime> query
        );
        Task<CursorResult<DateTime, NotificationDto>> GetUnreadNotificationsAsync(
            Guid userId,
            CursorFilter<DateTime> query
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
    }
}

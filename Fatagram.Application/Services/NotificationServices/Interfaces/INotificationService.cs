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

namespace Fatagram.Application.Services.NotificationServices.Interfaces
{
    public interface INotificationService
    {
        Task CreateAsync(Guid userId, NotificationDto notificationDto, bool isSave = true);
        Task<Result<CursorResult<NotificationDto, DateTime>>> GetAsync(
            Guid userId,
            CursorFilter<DateTime> query
        );
        Task<Result<CursorResult<NotificationDto, DateTime>>> GetUnreadAsync(
            Guid userId,
            CursorFilter<DateTime> query
        );
        Task<Result<int>> GetUnreadCountAsync(Guid userId);
        Task MarkAsReadAsync(Guid notificationId);
        Task MarkAllAsReadAsync(Guid userId);
        Task<Result> DeleteAsync(Guid notificationId);
        Task<Result> DeleteAllAsync(Guid userId);
    }
}

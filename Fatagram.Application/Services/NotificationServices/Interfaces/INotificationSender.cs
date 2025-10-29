using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.Application.Dtos.Notification;

namespace Fatagram.Application.Services.NotificationServices.Interfaces
{
    public interface INotificationSender
    {
        Task SendNotificationAsync(Guid userId, NotificationDto notification);
        Task SendNotificationToAllAsync(NotificationDto notification);
    }
}

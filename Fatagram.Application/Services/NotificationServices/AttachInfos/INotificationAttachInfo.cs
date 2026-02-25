using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.Application.Dtos.Notification;
using Fatagram.Domain.Models;

namespace Fatagram.Application.Services.NotificationServices.AttachInfos
{
    public interface INotificationAttachInfo
    {
        Task<NotificationDto> AttachAsync(NotificationDto notification);

        Task<List<NotificationDto>> AttachAsync(List<NotificationDto> notifications);
    }
}

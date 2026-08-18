using System;
using System.Collections.Generic;

namespace Fatagram.Application.Dtos.Notification
{
    public class BatchDeleteNotificationDto
    {
        public List<Guid> NotificationIds { get; set; } = new();
    }
}

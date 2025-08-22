using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fatagram.Application.Dtos.Notification
{
    public class NotificationsDto
    {
        public IEnumerable<NotificationDto> Notifications { get; set; } = new List<NotificationDto>();
        public int UnreadCount { get; set; } = 0;
    }
}
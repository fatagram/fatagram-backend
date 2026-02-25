using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.Domain.Enums.NotificationServices;

namespace Fatagram.Domain.Models
{
    public class Notification : BaseEntity
    {
        public Guid ActorId { get; set; }
        public ActorType ActorType { get; set; }
        public NotificationType Type { get; set; } = NotificationType.System;
        public NotificationTargetType TargetType { get; set; }
        public string? Data { get; set; }
        public Guid SourceId { get; set; }
        public ICollection<UserNotification> UserNotifications { get; set; } = null!;
    }

    public class UserNotification : BaseEntity
    {
        public Guid UserId { get; set; }
        public Guid NotificationId { get; set; }
        public bool IsRead { get; set; }
        public Notification Notification { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}

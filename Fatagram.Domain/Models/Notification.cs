using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.Domain.Enums.NotificationServices;

namespace Fatagram.Domain.Models
{
    public class Notification : BaseEntity
    {
        // ID
        [Key]
        [Column("id", TypeName = "uuid")]
        public Guid Id { get; set; }

        // User has this notification
        [Column("user_id", TypeName = "uuid")]
        public Guid UserId { get; set; }

        // Notification other data
        [Column("data", TypeName = "jsonb")]
        public Dictionary<string, string> Data { get; set; } = new Dictionary<string, string>();

        // Notification type
        [Column("type", TypeName = "smallint")]
        public NotificationType Type { get; set; }

        // Actor ID
        [Column("actor_id", TypeName = "uuid")]
        public Guid? ActorId { get; set; }

        // Link
        [Column("link", TypeName = "text")]
        public string Link { get; set; } = string.Empty;

        // Is read
        [Column("is_read", TypeName = "boolean")]
        public bool IsRead { get; set; } = false;
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.Application.Services.NotificationServices;

namespace Fatagram.Domain.Models
{
    public class Notification
    {
        [Key]
        [Column("id", TypeName = "uuid")]
        public Guid Id { get; set; }

        [Column("user_id", TypeName = "uuid")]
        public Guid UserId { get; set; }

        [Column("data", TypeName = "jsonb")]
        public Dictionary<string, string> Data { get; set; } = new Dictionary<string, string>();

        [Column("link", TypeName = "text")]
        public string Link { get; set; } = string.Empty;

        [Column("is_read", TypeName = "boolean")]
        public bool IsRead { get; set; } = false;

        [Column("created_at", TypeName = "timestamptz")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
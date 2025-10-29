using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fatagram.Domain.Enums.NotificationServices;

namespace Fatagram.Infrastructure.Projections
{
    public class NotificationProjection
    {
        public Guid Id { get; set; }
        public NotificationType Type { get; set; }
        public Dictionary<string, string> Data { get; set; } = new Dictionary<string, string>();
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }
        public Guid UserId { get; set; }
        public Guid? ActorId { get; set; }
        public string Link { get; set; } = string.Empty;
        public string LanguageCode { get; set; } = string.Empty;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fatagram.Domain.Enums.NotificationServices;

namespace Fatagram.Domain.Models
{
    public class NotificationContent : BaseEntity
    {
        public NotificationType Type { get; set; }
        public string LanguageCode { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public Language? Language { get; set; }
    }
}

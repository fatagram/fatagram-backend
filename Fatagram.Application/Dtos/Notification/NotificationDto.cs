using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Fatagram.Application.Services.NotificationServices;
using Fatagram.Domain.Enums.NotificationServices;
using Fatagram.Shared.Enums;
using Fatagram.Shared.Extensions;
using SixLabors.ImageSharp.PixelFormats;

namespace Fatagram.Application.Dtos.Notification
{
    public class NotificationDto
    {
        public string Id { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string SourceId { get; set; } = string.Empty;
        public Dictionary<string, string> Data { get; set; } = [];
        public string? ActorId { get; set; }
        public string ActorType { get; set; } = string.Empty;
        public string? ActorImageUrl { get; set; }
        public string? ActorName { get; set; }
        public string? Link { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public NotificationType Type { get; set; } = NotificationType.System;
        public bool IsRead { get; set; } = false;
        public DateTime? CreatedAt { get; set; }
    }
}

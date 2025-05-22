using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.Application.Services.NotificationServices;
using Fatagram.Shared.Enums;
using Fatagram.Shared.Extensions;
using SixLabors.ImageSharp.PixelFormats;

namespace Fatagram.Application.Dtos.Notification
{
    public class NotificationDto
    {
        public string UserId { get; set; } = string.Empty;
        public string SenderName { get; set; } = string.Empty;
        public Dictionary<string, string> Data { get; set; } = new Dictionary<string, string>();
        public string Link { get; set; } = string.Empty;
        public bool IsRead { get; set; } = false;
        public TimeDistance TimeDistance { get; set; } = new TimeDistance(0, TimeUnit.Miliseconds);
    }
}
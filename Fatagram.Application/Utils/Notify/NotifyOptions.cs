using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.Application.Dtos.Notification;

namespace Fatagram.Application.Utils
{
    /// <summary>
    /// Notification options carrying shared data for all channels.
    /// Each decorator reads the same DTO and handles its own channel.
    /// </summary>
    public record NotifyOptions(NotificationDto Notification, bool IsSave = true);
}

using System;
using Fatagram.Application.Dtos.Notification;

namespace Fatagram.Application.Utils.Notify;

/// <summary>
/// Notification options carrying shared data for all channels.
/// </summary>
public record NotifyOptions(NotificationDto Notification, bool IsSave = true);

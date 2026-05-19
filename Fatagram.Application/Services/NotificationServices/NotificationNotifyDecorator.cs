using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.Application.Dtos.Notification;
using Fatagram.Application.Services.NotificationServices.Interface;
using Fatagram.Application.Services.NotificationServices.Interfaces;
using Fatagram.Application.Utils;
using Fatagram.Application.Utils.Notify;
using Fatagram.Shared.Enums;
using Microsoft.Extensions.Logging;

namespace Fatagram.Application.Services.NotificationServices;

public class NotificationNotifyDecorator(
    INotify innerNotify,
    INotificationService notificationService,
    ILogger<NotificationNotifyDecorator> logger
) : NotifyDecorator(innerNotify)
{
    private readonly INotificationService _notificationService = notificationService;
    private readonly ILogger<NotificationNotifyDecorator> _logger = logger;

    public override async Task<Result> NotifyAsync(Guid userId, NotifyOptions options)
    {
        await base.NotifyAsync(userId, options);

        try
        {
            await _notificationService.CreateAsync(userId, options.Notification, options.IsSave);
            _logger.LogInformation("Notification created for user {UserId}", userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create notification for user {UserId}", userId);
            return Result.Create(
                ResponseStatusCode.InternalServerError,
                "Failed to create notification"
            );
        }

        return Result.Create();
    }
}

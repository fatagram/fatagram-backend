using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.Application.Dtos.Notification;
using Fatagram.Application.Services.NotificationServices.Interface;
using Fatagram.Application.Utils;
using Fatagram.Shared.Enums;
using Microsoft.Extensions.Logging;

namespace Fatagram.Application.Services.NotificationServices
{
    public class NotificationNotifyDecorator : NotifyDecorator
    {
        private readonly INotificationService _notificationService;
        private readonly ILogger<NotificationNotifyDecorator> _logger;

        public NotificationNotifyDecorator(
            INotify innerNotify,
            INotificationService notificationService,
            ILogger<NotificationNotifyDecorator> logger
        )
            : base(innerNotify)
        {
            _notificationService = notificationService;
            _logger = logger;
        }

        public override async Task<Result> NotifyAsync(Guid userId, NotifyOptions options)
        {
            await base.NotifyAsync(userId, options);

            try
            {
                await _notificationService.CreateNotificationAsync(
                    userId,
                    options.Notification,
                    options.IsSave
                );
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
}

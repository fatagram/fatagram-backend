using Fatagram.Application.Dtos.Notification;
using Fatagram.Application.Utils;
using Fatagram.Application.Utils.Notify;
using Microsoft.Extensions.Logging;

namespace Fatagram.Application.Services.NotificationServices;

/// <summary>
/// Decorator to send email notifications (placeholder for future implementation)
/// </summary>
public class EmailNotifyDecorator(INotify innerNotify, ILogger<EmailNotifyDecorator> logger)
    : NotifyDecorator(innerNotify)
{
    private readonly ILogger<EmailNotifyDecorator> _logger = logger;

    public override async Task<Result> NotifyAsync(Guid userId, NotifyOptions options)
    {
        var result = await base.NotifyAsync(userId, options);

        // TODO: Implement email sending logic
        _logger.LogInformation(
            "Email notification placeholder for user {UserId}, type {Type}",
            userId,
            options.Notification.Type
        );

        return result;
    }
}

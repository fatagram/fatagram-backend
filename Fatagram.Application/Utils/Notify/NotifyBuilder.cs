using Fatagram.Application.Dtos.Notification;
using Fatagram.Application.Services.NotificationServices;
using Fatagram.Application.Services.NotificationServices.Interface;
using Fatagram.Application.Services.NotificationServices.Interfaces;
using Fatagram.Application.Utils.Notify;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Fatagram.Application.Utils.Notify;

/// <summary>
/// Fluent builder to dynamically compose INotify decorator chain per call.
/// Each caller decides which decorators to wrap:
///
///   await _notifyBuilder
///       .WithPush()
///       .WithEmail()
///       .NotifyAsync(userId, options);
/// </summary>
public class NotifyBuilder(IServiceProvider serviceProvider)
{
    private readonly IServiceProvider _sp = serviceProvider;
    private INotify _notify = new BaseNotify();

    /// <summary>
    /// Wrap with push notification via SignalR
    /// </summary>
    public NotifyBuilder WithPush()
    {
        _notify = new NotificationNotifyDecorator(
            _notify,
            _sp.GetRequiredService<INotificationService>(),
            _sp.GetRequiredService<ILogger<NotificationNotifyDecorator>>()
        );
        return this;
    }

    /// <summary>
    /// Wrap with email notification (placeholder, implement later)
    /// </summary>
    public NotifyBuilder WithEmail()
    {
        _notify = new EmailNotifyDecorator(
            _notify,
            _sp.GetRequiredService<ILogger<EmailNotifyDecorator>>()
        );
        return this;
    }

    /// <summary>
    /// Execute the built decorator chain
    /// </summary>
    public Task<Result> NotifyAsync(Guid userId, NotifyOptions options)
    {
        return _notify.NotifyAsync(userId, options);
    }
}

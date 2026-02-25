using Fatagram.Application.Dtos.Notification;
using Fatagram.Application.Utils;
using Microsoft.Extensions.Logging;

namespace Fatagram.Application.Services.NotificationServices
{
    /// <summary>
    /// Decorator to send email notifications (placeholder for future implementation)
    /// </summary>
    public class EmailNotifyDecorator : NotifyDecorator
    {
        private readonly ILogger<EmailNotifyDecorator> _logger;

        public EmailNotifyDecorator(INotify innerNotify, ILogger<EmailNotifyDecorator> logger)
            : base(innerNotify)
        {
            _logger = logger;
        }

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
}

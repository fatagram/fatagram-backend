using Fatagram.Application.Dtos.Notification;

namespace Fatagram.Application.Utils.Notify;

public interface INotify
{
    Task<Result> NotifyAsync(Guid userId, NotifyOptions options);
}

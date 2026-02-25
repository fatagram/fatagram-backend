using Fatagram.Application.Dtos.Notification;

namespace Fatagram.Application.Utils
{
    /// <summary>
    /// Base implementation of INotify - innermost of decorator chain, does nothing
    /// </summary>
    public class BaseNotify : INotify
    {
        public Task<Result> NotifyAsync(Guid userId, NotifyOptions options)
        {
            return Task.FromResult(Result.Create());
        }
    }
}

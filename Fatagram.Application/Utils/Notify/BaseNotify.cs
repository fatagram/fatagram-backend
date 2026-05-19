namespace Fatagram.Application.Utils.Notify;

public class BaseNotify : INotify
{
    public Task<Result> NotifyAsync(Guid userId, NotifyOptions options)
    {
        return Task.FromResult(Result.Create());
    }
}

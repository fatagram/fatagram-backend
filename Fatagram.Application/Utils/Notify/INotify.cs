using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.Application.Dtos.Notification;

namespace Fatagram.Application.Utils
{
    public interface INotify
    {
        Task<Result> NotifyAsync(Guid userId, NotifyOptions options);
    }
}

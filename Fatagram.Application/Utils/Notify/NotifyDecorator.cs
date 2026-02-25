using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.Application.Dtos.Notification;

namespace Fatagram.Application.Utils
{
    public class NotifyDecorator : INotify
    {
        protected readonly INotify _innerNotify;

        protected NotifyDecorator(INotify innerNotify)
        {
            _innerNotify = innerNotify;
        }

        public virtual async Task<Result> NotifyAsync(Guid userId, NotifyOptions options)
        {
            return await _innerNotify.NotifyAsync(userId, options);
        }
    }
}

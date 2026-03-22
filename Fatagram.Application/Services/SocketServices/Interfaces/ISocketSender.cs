using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.Application.Dtos.Message;
using Fatagram.Application.Utils;

namespace Fatagram.Application.Services.SockerServices.Interfaces
{
    public interface ISocketSender<TPayload>
        where TPayload : class
    {
        Task SendAsync(Guid userId, SocketMessage<TPayload> message);
        Task SendAllAsync(IEnumerable<Guid> userIds, SocketMessage<TPayload> message);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.Application.Dtos.Message;
using Fatagram.Application.Utils;

namespace Fatagram.Application.Services.SocketServices
{
    public interface ISocketSender<TPayload>
        where TPayload : class
    {
        Task SendAsync(Guid userId, SocketMessage<TPayload> message);
        Task SendAllAsync(IEnumerable<Guid> userIds, SocketMessage<TPayload> message);
        Task SendToGroupAsync(string groupName, SocketMessage<TPayload> message);
        Task SendToGroupExceptAsync(
            string groupName,
            IEnumerable<string> excludedConnectionIds,
            SocketMessage<TPayload> message
        );

        Task JoinGroupAsync(string connectionId, string groupName);
        Task LeaveGroupAsync(string connectionId, string groupName);
    }
}

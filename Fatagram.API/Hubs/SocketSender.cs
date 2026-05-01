using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.Application.Services.SockerServices.Interfaces;
using Fatagram.Application.Utils;
using Microsoft.AspNetCore.SignalR;

namespace Fatagram.API.Hubs
{
    public class SocketSender<TPayload> : ISocketSender<TPayload>
        where TPayload : class
    {
        private readonly IHubContext<AppHub> _hubContext;

        public SocketSender(IHubContext<AppHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendAllAsync(IEnumerable<Guid> userIds, SocketMessage<TPayload> message)
        {
            var tasks = userIds.Select(userId =>
                _hubContext.Clients.User(userId.ToString()).SendAsync("ReceiveMessage", message)
            );
            await Task.WhenAll(tasks);
        }

        public async Task SendAsync(Guid userId, SocketMessage<TPayload> message)
        {
            await _hubContext.Clients.User(userId.ToString()).SendAsync("ReceiveMessage", message);
        }

        public async Task SendToGroupAsync(string groupName, SocketMessage<TPayload> message)
        {
            await _hubContext.Clients.Group(groupName).SendAsync("ReceiveMessage", message);
        }

        public async Task SendToGroupExceptAsync(
            string groupName,
            IEnumerable<string> excludedConnectionIds,
            SocketMessage<TPayload> message
        )
        {
            await _hubContext
                .Clients.GroupExcept(groupName, excludedConnectionIds)
                .SendAsync("ReceiveMessage", message);
        }

        public async Task JoinGroupAsync(string connectionId, string groupName)
        {
            await _hubContext.Groups.AddToGroupAsync(connectionId, groupName);
        }

        public async Task LeaveGroupAsync(string connectionId, string groupName)
        {
            await _hubContext.Groups.RemoveFromGroupAsync(connectionId, groupName);
        }
    }
}

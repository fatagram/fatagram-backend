using System.Security.Claims;
using Fatagram.API.Utils;
using Fatagram.Application.Services.SocketServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Fatagram.API.Hubs
{
    [Authorize]
    public class AppHub(ISocketReceiver receiver) : Hub
    {
        private readonly ISocketReceiver _receiver = receiver;
        protected string? UserId => Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        protected string ConnectionId => Context.ConnectionId;

        public override async Task OnConnectedAsync()
        {
            if (UserId != null)
            {
                await Groups.AddToGroupAsync(ConnectionId, UserId);
                await _receiver.OnConnectedAsync(UserId, ConnectionId);
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            if (UserId != null)
            {
                await Groups.RemoveFromGroupAsync(ConnectionId, UserId);
                await _receiver.OnDisconnectedAsync(UserId, ConnectionId, exception);
            }
            await base.OnDisconnectedAsync(exception);
        }

        public async Task HandleAction(string action, string targetId, object? data)
        {
            if (UserId != null)
            {
                await _receiver.OnReceiveActionAsync(UserId, ConnectionId, action, targetId, data);
            }
        }
    }
}

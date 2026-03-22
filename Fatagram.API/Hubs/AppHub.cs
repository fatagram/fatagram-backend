using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Fatagram.API.Hubs
{
    [Authorize]
    public class AppHub : Hub
    {
        protected string? UserId => Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        protected string ConnectionId => Context.ConnectionId;

        public override Task OnConnectedAsync()
        {
            if (UserId != null)
            {
                // Add the user to a group based on their user ID
                Groups.AddToGroupAsync(ConnectionId, UserId);
            }
            return base.OnConnectedAsync();
        }

        // DisconnectedAsync is called when a user disconnects from the hub
        public override Task OnDisconnectedAsync(Exception? exception)
        {
            if (UserId != null)
            {
                // Remove the user from the group when they disconnect
                Groups.RemoveFromGroupAsync(ConnectionId, UserId);
            }
            return base.OnDisconnectedAsync(exception);
        }
    }
}

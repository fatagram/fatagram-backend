using Fatagram.Application.Dtos.Notification;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Data;
using System.Security.Claims;

namespace Fatagram.API.Hubs
{
    [Authorize]
    public class NotificationHub : Hub
    {
        public override Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId != null)
            {
                // Add the user to a group based on their user ID
                Groups.AddToGroupAsync(Context.ConnectionId, userId);
            }
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId != null)
            {
                // Remove the user from the group when they disconnect
                Groups.RemoveFromGroupAsync(Context.ConnectionId, userId);
            }
            return base.OnDisconnectedAsync(exception);
        }
    }
}

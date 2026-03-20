using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Fatagram.API.Hubs.Messages
{
    [Authorize]
    public class MessageHub : BaseHub
    {
        public override Task OnConnectedAsync()
        {
            if (UserId != null)
            {
                Groups.AddToGroupAsync(ConnectionId, UserId);
            }
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            if (UserId != null)
            {
                Groups.RemoveFromGroupAsync(ConnectionId, UserId);
            }
            return base.OnDisconnectedAsync(exception);
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Data;

namespace Fatagram.API.Hubs
{
    public class NotificationHub : Hub
    {
        [Authorize]
        public async Task SendNotification(string message)
        {
            await Clients.All.SendAsync("ReceiveNotification", message);
        }
    }
}

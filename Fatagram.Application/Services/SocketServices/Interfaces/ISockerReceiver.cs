using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fatagram.Application.Services.SocketServices.Interfaces
{
    public interface ISockerReceiver
    {
        Task OnConnectedAsync(string userId, string connectionId);
        Task OnDisconnectedAsync(string userId, string connectionId, Exception? exception);

        Task OnReceiveActionAsync(
            string userId,
            string connectionId,
            string action,
            string targetId,
            object? data = null
        );
    }
}

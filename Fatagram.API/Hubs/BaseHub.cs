using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;

namespace Fatagram.API.Hubs
{
    public class BaseHub : Hub
    {
        protected string? UserId => Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        protected string ConnectionId => Context.ConnectionId;
    }
}

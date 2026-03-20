using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.Application.Dtos.Message;
using Fatagram.Application.Services.MessageServices.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace Fatagram.API.Hubs.Messages
{
    public class MessageSender(IHubContext<MessageHub> hubContext) : IMessageSender
    {
        private readonly IHubContext<MessageHub> _hubContext = hubContext;

        public async Task SendMessageAllAsync(ResponseMessageDto message, IEnumerable<Guid> userIds)
        {
            var tasks = userIds.Select(userId =>
                _hubContext.Clients.User(userId.ToString()).SendAsync("ReceiveMessage", message)
            );
            await Task.WhenAll(tasks);
        }

        public async Task SendMessageAsync(Guid userId, ResponseMessageDto message)
        {
            if (userId == Guid.Empty)
            {
                return;
            }
            await _hubContext.Clients.User(userId.ToString()).SendAsync("ReceiveMessage", message);
            Console.WriteLine($"Sent message to user {userId}: {message.Content}");
        }
    }
}

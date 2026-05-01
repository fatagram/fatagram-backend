using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.Application.Services.SockerServices.Interfaces;
using Fatagram.Application.Services.SocketServices.Interfaces;
using Fatagram.Application.Utils;

namespace Fatagram.API.Hubs
{
    public class SockerReceiver(ISocketSender<object> sender) : ISockerReceiver
    {
        private readonly ISocketSender<object> _sender = sender;

        public Task OnConnectedAsync(string userId, string connectionId)
        {
            return Task.CompletedTask;
        }

        public Task OnDisconnectedAsync(string userId, string connectionId, Exception? exception)
        {
            return Task.CompletedTask;
        }

        public async Task OnReceiveActionAsync(
            string userId,
            string connectionId,
            string action,
            string targetId,
            object? data
        )
        {
            switch (action)
            {
                case "JoinConversation":
                    await _sender.JoinGroupAsync(connectionId, targetId);
                    break;

                case "LeaveConversation":
                    await _sender.LeaveGroupAsync(connectionId, targetId);
                    break;

                case "StartTyping":
                    await _sender.SendToGroupExceptAsync(
                        targetId,
                        [connectionId],
                        new SocketMessage<object>
                        {
                            Event = "UserIsTyping",
                            Payload = new { UserId = userId, ConversationId = targetId },
                        }
                    );
                    break;

                case "StopTyping":
                    await _sender.SendToGroupExceptAsync(
                        targetId,
                        [connectionId],
                        new SocketMessage<object>
                        {
                            Event = "UserStoppedTyping",
                            Payload = new { UserId = userId, ConversationId = targetId },
                        }
                    );
                    break;
            }
        }
    }
}

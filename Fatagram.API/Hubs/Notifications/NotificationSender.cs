using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.Application.Dtos.Notification;
using Fatagram.Application.Services.NotificationServices.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace Fatagram.API.Hubs.Notifications
{
    public class NotificationSender : INotificationSender
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationSender"/> class.
        /// </summary>
        public NotificationSender(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        /// <summary>
        /// Sends a notification to a specific user via SignalR.
        /// </summary>
        /// <param name="notification"></param>
        /// <returns></returns>
        public async Task SendNotificationAsync(Guid userId, NotificationDto notification)
        {
            if (userId != Guid.Empty)
            {
                await _hubContext
                    .Clients.User(userId.ToString())
                    .SendAsync("ReceiveNotification", notification);
            }
        }

        /// <summary>
        /// Sends a notification to all connected users via SignalR.
        /// </summary>
        /// <param name="notification"></param>
        /// <returns></returns>
        public async Task SendNotificationToAllAsync(NotificationDto notification)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveNotification", notification);
        }
    }
}

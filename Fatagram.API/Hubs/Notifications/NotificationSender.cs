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

        public NotificationSender(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendNotificationAsync(NotificationDto notification)
        {
            var userId = notification.UserId;
            if (userId != null)
            {
                await _hubContext.Clients.User(userId).SendAsync("ReceiveNotification", notification);
            }
        }

        public async Task SendNotificationToAllAsync(NotificationDto notification)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveNotification", notification);
        }
    }
}
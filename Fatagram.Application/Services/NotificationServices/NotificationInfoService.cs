using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.Application.Dtos.Notification;
using Fatagram.Application.Services.NotificationServices.Interface;
using Fatagram.Application.Services.NotificationServices.Interfaces;
using Fatagram.Domain.Enums.NotificationServices;
using Fatagram.Domain.Models;
using Fatagram.Infrastructure.Repositories.UserRepository.Interface;
using Fatagram.Shared.Extensions;

namespace Fatagram.Application.Services.NotificationServices
{
    public class NotificationInfoService
    {
        private readonly IUserRepository _userRepository;

        public NotificationInfoService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<NotificationDto> AttachInfosToNotificationAsync(
            NotificationDto notification
        )
        {
            switch (notification.Type)
            {
                case NotificationType.FriendRequestAccepted:
                case NotificationType.NewFriendRequest:
                    if (notification.ActorId == null)
                        return notification;
                    var user = (
                        await _userRepository.GetAllAsync(
                            u => u.Id == notification.ActorId.ToGuid(),
                            u => new User { FullName = u.FullName, Avatar = u.Avatar }
                        )
                    ).FirstOrDefault();
                    notification.ActorName = user?.FullName;
                    notification.ActorImageUrl = user?.Avatar;
                    return notification;
                default:
                    return notification;
            }
        }
    }
}

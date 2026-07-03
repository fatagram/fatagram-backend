using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.Application.Dtos.Notification;
using Fatagram.Domain.Models;
using Fatagram.Application.Abstractions.Repositories;
using Fatagram.Shared.Extensions;
using FluentValidation.Validators;

namespace Fatagram.Application.Services.NotificationServices.AttachInfos
{
    public class FriendRequestAcceptedAttachInfo(IUserRepository userRepository)
        : INotificationAttachInfo
    {
        private readonly IUserRepository _userRepository = userRepository;

        public async Task<NotificationDto> AttachAsync(NotificationDto notification)
        {
            if (notification.ActorId == null || notification.ActorId == Guid.Empty.ToString())
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
        }

        public async Task<List<NotificationDto>> AttachAsync(List<NotificationDto> notifications)
        {
            var actorIds = notifications
                .Where(n => n.ActorId != null && n.ActorId != Guid.Empty.ToString())
                .Select(n => n.ActorId.ToGuid())
                .Distinct()
                .ToList();
            var users = await _userRepository.GetAllAsync(
                u => actorIds.Contains(u.Id),
                u => new User
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Avatar = u.Avatar,
                }
            );
            var userDict = users.ToDictionary(u => u.Id.ToString(), u => u);
            foreach (var notification in notifications)
            {
                if (
                    notification.ActorId != null
                    && userDict.TryGetValue(notification.ActorId, out var user)
                )
                {
                    notification.ActorName = user.FullName;
                    notification.ActorImageUrl = user.Avatar;
                }
            }
            return notifications;
        }
    }
}

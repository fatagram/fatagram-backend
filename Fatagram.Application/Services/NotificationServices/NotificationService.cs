using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Fatagram.Application.Dtos.Notification;
using Fatagram.Application.Services.NotificationServices.Interfaces;
using Fatagram.Domain.Models;
using Fatagram.Infrastructure.Repositories.NotificationRepository.Interface;
using Fatagram.Application.Utils;
using Fatagram.Shared.Extensions;

namespace Fatagram.Application.Services.NotificationServices.Interface
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly INotificationSender _notificationSender;
        private readonly NotificationInfoService _notificationInfoService;
        private readonly IMapper _mapper;

        public NotificationService(
            INotificationRepository notificationRepository,
            NotificationInfoService notificationInfoService,
            IMapper mapper,
            INotificationSender notificationSender)
        {
            _mapper = mapper;
            _notificationRepository = notificationRepository;
            _notificationSender = notificationSender;
            _notificationInfoService = notificationInfoService;
        }

        public async Task CreateNotificationAsync(NotificationDto notificationDto, bool isSave = true)
        {
            var attachedNotification = await _notificationInfoService.AttachInfosToNotificationAsync(notificationDto);
            if (isSave)
            {
                var notification = _mapper.Map<Notification>(attachedNotification);
                await _notificationRepository.AddAsync(notification);
                attachedNotification.Id = notification.Id.ToString();
            }    
            // Send notification to the user here
            await _notificationSender.SendNotificationAsync(attachedNotification);
        }

        public async Task DeleteAllNotificationsAsync(string userId)
        {
            var userIdGuid = Guid.Parse(userId);
            await _notificationRepository.DeleteAllNotificationsAsync(userIdGuid);
        }

        public async Task DeleteNotificationAsync(string notificationId)
        {
            var notificationIdGuid = Guid.Parse(notificationId);
            await _notificationRepository.DeleteNotificationAsync(notificationIdGuid);

            // Send cancel notification to the user
            // var cancelNotification = NotificationFactory.CreateCancelNotification()
        }

        public async Task<Result<NotificationsDto>> GetNotificationsAsync(string userId, int page, int pageSize)
        {
            var userIdGuid = userId.ToGuid();
            var data = await _notificationRepository.GetNotificationsAsync(userIdGuid, page, pageSize);

            var result = new List<NotificationDto>();
            foreach (var n in data.notifications)
            {
                var dto = _mapper.Map<NotificationDto>(n);
                result.Add(await _notificationInfoService.AttachInfosToNotificationAsync(dto));
            }

            return Result<NotificationsDto>.Success(new NotificationsDto
            {
                Notifications = result,
                UnreadCount = data.unreadCount
            });
        }

        public async Task<Result<NotificationsDto>> GetUnreadNotificationsAsync(string userId, int page, int pageSize)
        {
            var userIdGuid = userId.ToGuid();
            var data = await _notificationRepository.GetUnreadNotificationsAsync(userIdGuid, page, pageSize);

            var notificationsWithInfo = await Task.WhenAll(
                data.notifications.Select(async n => 
                    await _notificationInfoService.AttachInfosToNotificationAsync(_mapper.Map<NotificationDto>(n))));

            return Result<NotificationsDto>.Success(new NotificationsDto
            {
                Notifications = notificationsWithInfo,
                UnreadCount = data.unreadCount
            });
        }

        public async Task MarkAllNotificationsAsReadAsync(string userId)
        {
            var userIdGuid = Guid.Parse(userId);
            await _notificationRepository.MarkAllNotificationsAsReadAsync(userIdGuid);
        }

        public async Task MarkNotificationAsReadAsync(string notificationId)
        {
            var notificationIdGuid = Guid.Parse(notificationId);
            await _notificationRepository.MarkNotificationAsReadAsync(notificationIdGuid);
        }

        private async Task<IEnumerable<Notification>> FindNotificationAsync(string userId, string actorId, NotificationType type, Dictionary<string, string>? data)
        {
            return await _notificationRepository.FindNotifications(userId.ToGuid(), actorId.ToGuid(), type, data ?? new());
        }

        public async Task DeleteNotificationsAsync(
            string userId,
            string actorId,
            NotificationType type,
            Dictionary<string, string>? data = null,
            bool isSendCancel = true
        )
        {
            var notifications = await FindNotificationAsync(userId, actorId, type, data);
            foreach (var notification in notifications)
            {
                await _notificationRepository.DeleteNotificationAsync(notification.Id);
                if (isSendCancel)
                {
                    var cancelNotification = NotificationFactory.CreateCancelNotification(userId, notification.Id.ToString());
                    await _notificationSender.SendNotificationAsync(cancelNotification);
                }
            }
        }
    }
}
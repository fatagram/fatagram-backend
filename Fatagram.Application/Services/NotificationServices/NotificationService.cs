using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Fatagram.Application.Dtos.Notification;
using Fatagram.Domain.Models;
using Fatagram.Infrastructure.Repositories.NotificationRepository.Interface;

namespace Fatagram.Application.Services.NotificationServices.Interface
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IMapper _mapper;

        public NotificationService(INotificationRepository notificationRepository, IMapper mapper)
        {
            _mapper = mapper;
            _notificationRepository = notificationRepository;
        }

        public async Task CreateNotificationAsync(NotificationDto notificationDto)
        {
            var notification = _mapper.Map<Notification>(notificationDto);
            await _notificationRepository.AddAsync(notification);

            // Send notification to the user here
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
        }

        public async Task<IEnumerable<NotificationDto>> GetNotificationsAsync(string userId, int page, int pageSize)
        {
            var userIdGuid = Guid.Parse(userId);
            var notifications = await _notificationRepository.GetNotificationsAsync(userIdGuid, page, pageSize);
            return notifications.Select(n => _mapper.Map<NotificationDto>(n));
        }

        public async Task<IEnumerable<NotificationDto>> GetUnreadNotificationsAsync(string userId, int page, int pageSize)
        {
            var userIdGuid = Guid.Parse(userId);
            var notifications = await _notificationRepository.GetUnreadNotificationsAsync(userIdGuid, page, pageSize);
            return notifications.Select(n => _mapper.Map<NotificationDto>(n));
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
    }
}
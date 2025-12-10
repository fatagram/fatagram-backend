using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Fatagram.Application.Dtos.Filter;
using Fatagram.Application.Dtos.Notification;
using Fatagram.Application.Services.NotificationServices.Interfaces;
using Fatagram.Application.Utils;
using Fatagram.Domain.Enums.NotificationServices;
using Fatagram.Domain.Models;
using Fatagram.Infrastructure.Repositories.NotificationRepository.Interface;
using Fatagram.Infrastructure.Repositories.UserRepository.Interface;
using Fatagram.Shared.Extensions;

namespace Fatagram.Application.Services.NotificationServices.Interface
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly INotificationContentRepository _notificationContentRepository;
        private readonly INotificationSender _notificationSender;
        private readonly IUserRepository _userRepository;
        private readonly NotificationInfoService _notificationInfoService;
        private readonly IMapper _mapper;

        public NotificationService(
            INotificationRepository notificationRepository,
            INotificationContentRepository notificationContentRepository,
            NotificationInfoService notificationInfoService,
            IUserRepository userRepository,
            IMapper mapper,
            INotificationSender notificationSender
        )
        {
            _mapper = mapper;
            _notificationRepository = notificationRepository;
            _notificationContentRepository = notificationContentRepository;
            _notificationSender = notificationSender;
            _notificationInfoService = notificationInfoService;
            _userRepository = userRepository;
        }

        public async Task CreateNotificationAsync(
            Guid userId,
            NotificationDto notificationDto,
            bool isSave = true
        )
        {
            var attachedNotification =
                await _notificationInfoService.AttachInfosToNotificationAsync(notificationDto);
            if (isSave)
            {
                var notification = _mapper.Map<Notification>(attachedNotification);
                await _notificationRepository.AddAsync(notification);
                attachedNotification.Id = notification.Id.ToString();
            }
            // Send notification to the user here
            var userLang = await _userRepository.GetAsync(
                attachedNotification.UserId.ToGuid(),
                u => u.Language
            );
            var notificationContent = (
                await _notificationContentRepository.GetAllAsync(
                    n => n.Type == attachedNotification.Type && n.Language == userLang,
                    n => n.Content
                )
            ).FirstOrDefault();
            attachedNotification.Content = notificationContent;
            await _notificationSender.SendNotificationAsync(userId, attachedNotification);
        }

        public Task DeleteAllNotificationsAsync(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task DeleteNotificationAsync(Guid notificationId)
        {
            throw new NotImplementedException();
        }

        public Task<CursorResult<Guid?, NotificationDto>> GetNotificationsAsync(
            Guid userId,
            CursorFilter<Guid> query
        )
        {
            throw new NotImplementedException();
        }

        public Task<CursorResult<Guid?, NotificationDto>> GetUnreadNotificationsAsync(
            Guid userId,
            CursorFilter<Guid> query
        )
        {
            throw new NotImplementedException();
        }

        public Task MarkNotificationAsReadAsync(Guid notificationId)
        {
            throw new NotImplementedException();
        }

        public Task MarkAllNotificationsAsReadAsync(Guid userId)
        {
            throw new NotImplementedException();
        }

        private Task<IEnumerable<Notification>> FindNotificationAsync(
            Guid userId,
            Guid actorId,
            NotificationType type,
            Dictionary<string, string>? data
        )
        {
            throw new NotImplementedException();
        }

        public Task DeleteNotificationsAsync(
            Guid userId,
            Guid actorId,
            NotificationType type,
            Dictionary<string, string>? data = null,
            bool isSendCancel = true
        )
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
using Fatagram.Shared.Enums;
using Fatagram.Shared.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;

namespace Fatagram.Application.Services.NotificationServices.Interface
{
    public class NotificationService(
        INotificationRepository notificationRepository,
        NotificationInfoService notificationInfoService,
        IUserRepository userRepository,
        IMapper mapper,
        INotificationSender notificationSender,
        IUserNotificationRepository userNotificationRepository,
        ILogger<NotificationService> logger
    ) : INotificationService
    {
        private readonly INotificationRepository _notificationRepository = notificationRepository;
        private readonly INotificationSender _notificationSender = notificationSender;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly NotificationInfoService _notificationInfoService = notificationInfoService;
        private readonly IUserNotificationRepository _userNotificationRepository =
            userNotificationRepository;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<NotificationService> _logger = logger;

        public async Task CreateNotificationAsync(
            Guid userId,
            NotificationDto notificationDto,
            bool isSave = true
        )
        {
            var attachedNotification =
                await _notificationInfoService.AttachInfosToNotificationAsync(notificationDto);

            attachedNotification.CreatedAt = DateTime.UtcNow;

            if (isSave)
            {
                var notification = _mapper.Map<Notification>(attachedNotification);
                var savedNoti = await _notificationRepository.AddAsync(notification);

                if (savedNoti == null)
                {
                    _logger.LogError("Failed to save notification for user {UserId}", userId);
                    return;
                }

                var userNotification = new UserNotification
                {
                    UserId = userId,
                    NotificationId = savedNoti.Id,
                    IsRead = false,
                };
                var savedUserNotification = await _userNotificationRepository.AddAsync(
                    userNotification
                );
                attachedNotification.Id = userNotification.Id.ToString();
                attachedNotification.CreatedAt = savedUserNotification.CreatedAt;
            }

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

        public async Task<CursorResult<DateTime, NotificationDto>> GetNotificationsAsync(
            Guid userId,
            CursorFilter<DateTime> query
        )
        {
            _logger.LogInformation(
                "GetNotificationsAsync - UserId: {UserId}, Cursor: {Cursor}, Limit: {Limit}",
                userId,
                query.Cursor,
                query.Limit
            );

            var userNotifications = await _userNotificationRepository.GetAllAsync<
                UserNotification,
                DateTime
            >(
                un => un.UserId == userId,
                un => un.Notification.CreatedAt,
                true, // orderDesc = true để hiển thị notification mới nhất trước
                query.Limit,
                query.Cursor,
                un => un.Include(un => un.Notification)
            );

            _logger.LogInformation(
                "GetNotificationsAsync - Found {Count} notifications",
                userNotifications.Count
            );

            var notificationDtos = userNotifications
                .Select(un =>
                {
                    var dto = _mapper.Map<NotificationDto>(un);
                    dto.IsRead = un.IsRead;
                    return dto;
                })
                .ToList();

            var attachedNotifications =
                await _notificationInfoService.AttachInfosToNotificationsAsync(notificationDtos);

            var lastNotification = userNotifications.LastOrDefault();
            return CursorResult<DateTime, NotificationDto>.Create(
                attachedNotifications!,
                lastNotification?.Notification.CreatedAt,
                userNotifications.Count == query.Limit
            );
        }

        public Task<CursorResult<DateTime, NotificationDto>> GetUnreadNotificationsAsync(
            Guid userId,
            CursorFilter<DateTime> query
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

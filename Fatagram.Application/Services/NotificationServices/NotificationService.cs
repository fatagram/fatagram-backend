using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Fatagram.Application.Dtos.Filter;
using Fatagram.Application.Dtos.Notification;
using Fatagram.Application.Services.NotificationServices.Interfaces;
using Fatagram.Application.Services.SockerServices.Interfaces;
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
        ISocketSender<NotificationDto> socketSender,
        IMapper mapper,
        IUserNotificationRepository userNotificationRepository,
        ILogger<NotificationService> logger
    ) : INotificationService
    {
        private readonly INotificationRepository _notificationRepository = notificationRepository;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly NotificationInfoService _notificationInfoService = notificationInfoService;
        private readonly IUserNotificationRepository _userNotificationRepository =
            userNotificationRepository;
        private readonly ISocketSender<NotificationDto> _socketSender = socketSender;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<NotificationService> _logger = logger;

        public async Task CreateAsync(
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

            await _socketSender.SendAsync(
                userId,
                new SocketMessage<NotificationDto>
                {
                    Event = "NewNotification",
                    Payload = attachedNotification,
                }
            );
        }

        public async Task<Result<CursorResult<NotificationDto, DateTime>>> GetAsync(
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
                un => un.UserId == userId && !un.DeletedAt.HasValue,
                un => un.Notification.CreatedAt,
                true,
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
            return Result<CursorResult<NotificationDto, DateTime>>.Create(
                ResponseStatusCode.Success,
                new CursorResult<NotificationDto, DateTime>(
                    attachedNotifications!,
                    lastNotification?.Notification.CreatedAt,
                    userNotifications.Count == query.Limit
                ),
                "Get notifications successfully"
            );
        }

        public async Task MarkAsReadAsync(Guid notificationId)
        {
            var userNotification = await _userNotificationRepository.GetAsync(
                notificationId,
                s => s
            );

            if (userNotification is null)
            {
                return;
            }

            userNotification.IsRead = true;
            await _userNotificationRepository.UpdateAsync(userNotification);
        }

        public async Task MarkAllAsReadAsync(Guid userId)
        {
            await _userNotificationRepository.UpdateAsync(
                un => un.UserId == userId && !un.IsRead,
                un => un.IsRead = true
            );
        }

        public async Task<Result> DeleteAllAsync(Guid userId)
        {
            await _userNotificationRepository.SoftDeleteRangeAsync(un => un.UserId == userId);
            return Result.Create();
        }

        public async Task<Result> DeleteAsync(Guid notificationId)
        {
            await _userNotificationRepository.SoftDeleteAsync(notificationId);
            return Result.Create();
        }

        public Task<Result<CursorResult<NotificationDto, DateTime>>> GetUnreadAsync(
            Guid userId,
            CursorFilter<DateTime> query
        )
        {
            throw new NotImplementedException();
        }

        public async Task<Result<int>> GetUnreadCountAsync(Guid userId)
        {
            var count = await _userNotificationRepository.CountAsync(un =>
                un.UserId == userId && !un.IsRead && un.DeletedAt == null
            );
            return Result<int>.Create(ResponseStatusCode.Success, count);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Threading.Tasks;
using Fatagram.API.Utils;
using Fatagram.Application.Dtos.Filter;
using Fatagram.Application.Dtos.Notification;
using Fatagram.Application.Exceptions.MiddleLevelExceptions;
using Fatagram.Application.Services.NotificationServices;
using Fatagram.Shared.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace Fatagram.API.Controllers.V1
{
    [Authorize]
    public class NotificationController(
        INotificationService notificationService,
        ILogger<NotificationController> logger
    ) : BaseApiController
    {
        private readonly INotificationService _notificationService = notificationService;
        private readonly ILogger<NotificationController> _logger = logger;

        /// <summary>
        /// Get notifications for the authenticated user with pagination support.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        /// <exception cref="UnauthorizedException"></exception>
        [HttpGet]
        public async Task<IActionResult> GetNotifications([FromQuery] CursorFilter<DateTime> filter)
        {
            _logger.LogInformation(
                "GetNotifications - Cursor: {Cursor}, Limit: {Limit}",
                filter.Cursor,
                filter.Limit
            );
            var userId = GetCurrentUserId();
            _logger.LogInformation("User ID: {UserId}", userId);
            var result = await _notificationService.GetAsync(userId, filter);
            return result.ToActionResult();
        }

        /// <summary>
        /// Get unread notifications for the authenticated user with pagination support.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        /// <exception cref="UnauthorizedException"></exception>
        [HttpGet("unread")]
        public async Task<IActionResult> GetUnreadNotifications(
            [FromQuery] CursorFilter<DateTime> filter
        )
        {
            var userId = GetCurrentUserId();
            var result = await _notificationService.GetUnreadAsync(userId, filter);
            return result.ToActionResult();
        }

        [HttpGet("unread-count")]
        public async Task<IActionResult> GetUnreadNotificationCount()
        {
            var userId = GetCurrentUserId();
            var result = await _notificationService.GetUnreadCountAsync(userId);
            return result.ToActionResult();
        }

        /// <summary>
        /// Mark a specific notification as read for the authenticated user.
        /// </summary>
        /// <param name="notificationId"></param>
        /// <returns></returns>
        /// <exception cref="UnauthorizedException"></exception>
        [HttpPost("{notificationId}/read")]
        public async Task<IActionResult> MarkNotificationAsRead(Guid notificationId)
        {
            await _notificationService.MarkAsReadAsync(notificationId);
            return Ok();
        }

        /// <summary>
        /// Mark all notifications as read for the authenticated user.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="UnauthorizedException"></exception>
        [HttpPost("read-all")]
        public async Task<IActionResult> MarkAllNotificationsAsRead()
        {
            var userId = GetCurrentUserId();
            await _notificationService.MarkAllAsReadAsync(userId);
            return Ok();
        }

        /// <summary>
        /// Delete a specific notification for the authenticated user.
        /// </summary>
        /// <param name="notificationId"></param>
        /// <returns></returns>
        /// <exception cref="UnauthorizedException"></exception>
        [HttpDelete("{notificationId}")]
        public async Task<IActionResult> DeleteNotification(Guid notificationId)
        {
            var userId = GetCurrentUserId();
            return (await _notificationService.DeleteAsync(userId, notificationId)).ToActionResult();
        }

        /// <summary>
        /// Delete multiple notifications for the authenticated user.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("batch-delete")]
        public async Task<IActionResult> DeleteBatchNotifications(
            [FromBody] BatchDeleteNotificationDto request
        )
        {
            var userId = GetCurrentUserId();
            return (
                await _notificationService.DeleteRangeAsync(userId, request.NotificationIds)
            ).ToActionResult();
        }

        /// <summary>
        /// Delete all notifications for the authenticated user.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="UnauthorizedException"></exception>
        [HttpDelete]
        public async Task<IActionResult> DeleteAllNotifications()
        {
            var userId = GetCurrentUserId();
            return (await _notificationService.DeleteAllAsync(userId)).ToActionResult();
        }
    }
}

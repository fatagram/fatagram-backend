using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Threading.Tasks;
using Fatagram.API.Utils;
using Fatagram.Application.Dtos.Filter;
using Fatagram.Application.Exceptions.MiddleLevelExceptions;
using Fatagram.Application.Services.NotificationServices.Interface;
using Fatagram.Shared.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace Fatagram.API.Controllers.V1
{
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
        [Authorize]
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
            var result = await _notificationService.GetNotificationsAsync(userId, filter);
            return result.ToActionResult();
        }

        /// <summary>
        /// Get unread notifications for the authenticated user with pagination support.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        /// <exception cref="UnauthorizedException"></exception>
        [Authorize]
        [HttpGet("unread")]
        public async Task<IActionResult> GetUnreadNotifications(
            [FromQuery] CursorFilter<DateTime> filter
        )
        {
            var userId = GetCurrentUserId();
            var result = await _notificationService.GetUnreadNotificationsAsync(userId, filter);
            return result.ToActionResult();
        }

        /// <summary>
        /// Mark a specific notification as read for the authenticated user.
        /// </summary>
        /// <param name="notificationId"></param>
        /// <returns></returns>
        /// <exception cref="UnauthorizedException"></exception>
        [Authorize]
        [HttpPost("read/{notificationId}")]
        public async Task<IActionResult> MarkNotificationAsRead(Guid notificationId)
        {
            await _notificationService.MarkNotificationAsReadAsync(notificationId);
            return Ok();
        }

        /// <summary>
        /// Mark all notifications as read for the authenticated user.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="UnauthorizedException"></exception>
        [Authorize]
        [HttpPost("read/all")]
        public async Task<IActionResult> MarkAllNotificationsAsRead()
        {
            var userId = GetCurrentUserId();
            await _notificationService.MarkAllNotificationsAsReadAsync(userId);
            return Ok();
        }

        /// <summary>
        /// Delete a specific notification for the authenticated user.
        /// </summary>
        /// <param name="notificationId"></param>
        /// <returns></returns>
        /// <exception cref="UnauthorizedException"></exception>
        [Authorize]
        [HttpDelete("{notificationId}")]
        public async Task<IActionResult> DeleteNotification(Guid notificationId)
        {
            await _notificationService.DeleteNotificationAsync(notificationId);
            return Ok();
        }

        /// <summary>
        /// Delete all notifications for the authenticated user.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="UnauthorizedException"></exception>
        [Authorize]
        [HttpDelete("all")]
        public async Task<IActionResult> DeleteAllNotifications()
        {
            var userId = GetCurrentUserId();
            await _notificationService.DeleteAllNotificationsAsync(userId);
            return Ok();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Threading.Tasks;
using Fatagram.API.Utils;
using Fatagram.Application.Dtos.Query;
using Fatagram.Application.Exceptions.MiddleLevelExceptions;
using Fatagram.Application.Services.NotificationServices.Interface;
using Fatagram.Shared.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace Fatagram.API.Controllers.V1
{
    [Route("api/[controller]")]
    public class NotificationController : BaseApiController
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        /// <summary>
        /// Get notifications for the authenticated user with pagination support.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        /// <exception cref="UnauthorizedException"></exception>
        [Authorize]
        [HttpGet("getNotifications")]
        public async Task<IActionResult> GetNotifications(CursorQuery<Guid?> query)
        {
            var userId = GetCurrentUserId();
            var result = await _notificationService.GetNotificationsAsync(userId, query);
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
        [HttpGet("getUnreadNotifications")]
        public async Task<IActionResult> GetUnreadNotifications(CursorQuery<Guid?> query)
        {
            var userId = GetCurrentUserId();
            var result = await _notificationService.GetUnreadNotificationsAsync(userId, query);
            return result.ToActionResult();
        }

        /// <summary>
        /// Mark a specific notification as read for the authenticated user.
        /// </summary>
        /// <param name="notificationId"></param>
        /// <returns></returns>
        /// <exception cref="UnauthorizedException"></exception>
        [Authorize]
        [HttpPost("markNotificationAsRead/{notificationId}")]
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
        [HttpPost("markAllNotificationsAsRead")]
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
        [HttpDelete("deleteNotification")]
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
        [HttpDelete("deleteAllNotifications")]
        public async Task<IActionResult> DeleteAllNotifications()
        {
            var userId = GetCurrentUserId();
            await _notificationService.DeleteAllNotificationsAsync(userId);
            return Ok();
        }
    }
}

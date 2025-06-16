using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Fatagram.API.Utils;
using Fatagram.Application.Exceptions.MiddleLevelExceptions;
using Fatagram.Application.Services.NotificationServices.Interface;
using Microsoft.AspNetCore.Authorization;

namespace Fatagram.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [Authorize]
        [HttpGet("getNotifications")]
        public async Task<IActionResult> GetNotifications(int page, int pageSize)
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                throw new UnauthorizedException();
            }
            var result = await _notificationService.GetNotificationsAsync(userId ?? "", page, pageSize);
            return result.ToActionResult();
        }

        [Authorize]
        [HttpGet("getUnreadNotifications")]
        public async Task<IActionResult> GetUnreadNotifications(int page, int pageSize)
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                throw new UnauthorizedException();
            }
            var result = await _notificationService.GetUnreadNotificationsAsync(userId ?? "", page, pageSize);
            return result.ToActionResult();
        }

        [Authorize]
        [HttpPost("markNotificationAsRead/{notificationId}")]
        public async Task<IActionResult> MarkNotificationAsRead(string notificationId)
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                throw new UnauthorizedException();
            }
            await _notificationService.MarkNotificationAsReadAsync(notificationId);
            return Ok();
        }

        [Authorize]
        [HttpPost("markAllNotificationsAsRead")]
        public async Task<IActionResult> MarkAllNotificationsAsRead()
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                throw new UnauthorizedException();
            }
            await _notificationService.MarkAllNotificationsAsReadAsync(userId);
            return Ok();
        }

        [Authorize]
        [HttpDelete("deleteNotification")]
        public async Task<IActionResult> DeleteNotification(string notificationId)
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                throw new UnauthorizedException();
            }
            await _notificationService.DeleteNotificationAsync(notificationId);
            return Ok();
        }

        [Authorize]
        [HttpDelete("deleteAllNotifications")]
        public async Task<IActionResult> DeleteAllNotifications()
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                throw new UnauthorizedException();
            }
            await _notificationService.DeleteAllNotificationsAsync(userId);
            return Ok();
        }
    }
}
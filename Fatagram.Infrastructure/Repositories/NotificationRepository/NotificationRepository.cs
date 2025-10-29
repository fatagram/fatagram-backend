using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.Domain.Enums.NotificationServices;
using Fatagram.Domain.Models;
using Fatagram.Infrastructure.Data;
using Fatagram.Infrastructure.Projections;
using Fatagram.Infrastructure.Repositories.NotificationRepository.Interface;
using Fatagram.Infrastructure.Utils.Query;
using Microsoft.EntityFrameworkCore;

namespace Fatagram.Infrastructure.Repositories.NotificationRepository
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly AppDbContext _context;

        public NotificationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Notification notification)
        {
            await _context.Notifications.AddAsync(notification);
            await _context.SaveChangesAsync();
        }

        public Task DeleteAllNotificationsAsync(Guid userId)
        {
            var notifications = _context.Notifications.Where(n => n.UserId == userId);
            _context.Notifications.RemoveRange(notifications);
            return _context.SaveChangesAsync();
        }

        public async Task DeleteNotificationAsync(Guid notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification != null)
            {
                _context.Notifications.Remove(notification);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<(
            IEnumerable<NotificationProjection> notifications,
            int total
        )> GetNotificationsAsync(Guid userId, string langCode, CursorQuery<Guid> query)
        {
            var dbQuery = _context
                .Notifications.Where(n => n.UserId == userId) // chỉ lấy noti của user
                .OrderByDescending(n => n.CreatedAt)
                .ThenByDescending(n => n.Id)
                .AsQueryable();

            if (query?.Cursor != null)
            {
                var cursor = await _context.Notifications.FindAsync(query.Cursor);
                if (cursor != null)
                {
                    dbQuery = dbQuery.Where(n =>
                        n.CreatedAt < cursor.CreatedAt
                        || (n.CreatedAt == cursor.CreatedAt && n.Id.CompareTo(cursor.Id) < 0)
                    );
                }
            }

            var notifications = await dbQuery
                .Take(query?.Limit ?? 10)
                .Select(n => new NotificationProjection
                {
                    Id = n.Id,
                    UserId = n.UserId,
                    Data = n.Data,
                    Type = n.Type,
                    ActorId = n.ActorId,
                    Link = n.Link,
                    IsRead = n.IsRead,
                    CreatedAt = n.CreatedAt,
                    Content =
                        _context
                            .NotificationContents.Where(c =>
                                c.Type == n.Type && c.LanguageCode == langCode
                            )
                            .Select(c => c.Content)
                            .FirstOrDefault()
                        ?? string.Empty,
                })
                .ToListAsync();

            var unreadCount = await _context.Notifications.CountAsync(n =>
                n.UserId == userId && !n.IsRead
            );

            return (notifications, unreadCount);
        }

        public async Task<(
            IEnumerable<NotificationProjection> notifications,
            int total
        )> GetUnreadNotificationsAsync(Guid userId, string langCode, CursorQuery<Guid> query)
        {
            var dbQuery = _context
                .Notifications.Where(n => n.UserId == userId && !n.IsRead)
                .OrderByDescending(n => n.CreatedAt)
                .ThenByDescending(n => n.Id)
                .AsQueryable();

            if (query?.Cursor != null)
            {
                var cursor = await _context.Notifications.FindAsync(query.Cursor);
                if (cursor != null)
                {
                    dbQuery = dbQuery.Where(n =>
                        n.CreatedAt < cursor.CreatedAt
                        || (n.CreatedAt == cursor.CreatedAt && n.Id.CompareTo(cursor.Id) < 0)
                    );
                }
            }

            var notifications = await dbQuery
                .Take(query?.Limit ?? 10)
                .Select(n => new NotificationProjection
                {
                    Id = n.Id,
                    UserId = n.UserId,
                    Data = n.Data,
                    Type = n.Type,
                    ActorId = n.ActorId,
                    Link = n.Link,
                    IsRead = n.IsRead,
                    CreatedAt = n.CreatedAt,
                    Content =
                        _context
                            .NotificationContents.Where(c =>
                                c.Type == n.Type && c.LanguageCode == langCode
                            )
                            .Select(c => c.Content)
                            .FirstOrDefault()
                        ?? string.Empty,
                })
                .ToListAsync();

            var unreadCount = await _context.Notifications.CountAsync(n =>
                n.UserId == userId && !n.IsRead
            );

            return (notifications, unreadCount);
        }

        public async Task MarkAllNotificationsAsReadAsync(Guid userId)
        {
            var notifications = _context.Notifications.Where(n => n.UserId == userId && !n.IsRead);
            foreach (var notification in notifications)
            {
                notification.IsRead = true;
            }
            await _context.SaveChangesAsync();
        }

        public async Task MarkNotificationAsReadAsync(Guid notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification != null)
            {
                notification.IsRead = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Notification>> FindNotifications(
            Guid userId,
            Guid actorId,
            NotificationType type,
            Dictionary<string, string> data
        )
        {
            var notifications = await _context
                .Notifications.Where(n =>
                    n.UserId == userId && n.Type == type && n.ActorId == actorId
                )
                .ToListAsync();

            if (data != null && data.Count > 0)
            {
                return notifications
                    .Where(n =>
                        data.All(d => n.Data.ContainsKey(d.Key) && n.Data[d.Key] == d.Value)
                    )
                    .ToList();
            }
            return notifications;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Fatagram.Domain.Enums.NotificationServices;
using Fatagram.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Fatagram.Infrastructure.Data.Extensions
{
    public static class NotificationContentExtension
    {
        public static void AddNotificationContent(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<NotificationContent>(entity =>
            {
                entity.ConfigureBaseEntity();
                entity.ToTable("notification_contents");
                entity
                    .Property(e => e.LanguageCode)
                    .HasColumnName("language_code")
                    .HasColumnType("VARCHAR(2)")
                    .IsRequired();
                entity
                    .Property(e => e.Type)
                    .HasColumnName("type")
                    .HasColumnType("SMALLINT")
                    .HasConversion<int>()
                    .IsRequired();
                entity
                    .Property(e => e.Content)
                    .HasColumnName("content")
                    .HasColumnType("TEXT")
                    .IsRequired();

                // Relationships
                entity
                    .HasOne(e => e.Language)
                    .WithMany(l => l.NotificationContents)
                    .HasForeignKey(e => e.LanguageCode)
                    .HasPrincipalKey(l => l.Code)
                    .OnDelete(DeleteBehavior.Restrict);

                // Seed data (use fixed deterministic timestamps to avoid non-deterministic model changes)
                var seedDate = new DateTime(2025, 01, 01, 0, 0, 0, DateTimeKind.Utc);
                entity.HasData(
                    new NotificationContent
                    {
                        Id = new Guid("11111111-1111-1111-1111-111111111111"),
                        Type = NotificationType.NewFriendRequest,
                        LanguageCode = "en",
                        Content = "{actorName} sent you a friend request.",
                        CreatedAt = seedDate,
                    },
                    new NotificationContent
                    {
                        Id = new Guid("22222222-2222-2222-2222-222222222222"),
                        Type = NotificationType.FriendRequestAccepted,
                        LanguageCode = "en",
                        Content = "{actorName} accepted your friend request.",
                        CreatedAt = seedDate,
                    },
                    new NotificationContent
                    {
                        Id = new Guid("33333333-3333-3333-3333-333333333333"),
                        Type = NotificationType.NewFriendRequest,
                        LanguageCode = "vi",
                        Content = "{actorName} đã gửi cho bạn một lời mời kết bạn.",
                        CreatedAt = seedDate,
                    },
                    new NotificationContent
                    {
                        Id = new Guid("44444444-4444-4444-4444-444444444444"),
                        Type = NotificationType.FriendRequestAccepted,
                        LanguageCode = "vi",
                        Content = "{actorName} đã chấp nhận lời mời kết bạn của bạn.",
                        CreatedAt = seedDate,
                    }
                );
            });
        }
    }
}

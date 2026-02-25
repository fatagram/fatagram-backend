using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Fatagram.Domain.Enums.NotificationServices;
using Fatagram.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Fatagram.Infrastructure.Data.Extensions
{
    public static class NotificationExtension
    {
        public static void AddNotification(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Notification>(entity =>
            {
                entity.ConfigureBaseEntity();
                entity.ToTable("notifications");
                entity.Property(e => e.ActorId).HasColumnName("actor_id").HasColumnType("uuid");
                entity
                    .Property(e => e.ActorType)
                    .HasColumnName("actor_type")
                    .HasConversion<int>()
                    .IsRequired();
                entity
                    .Property(e => e.Type)
                    .HasColumnName("notification_type")
                    .HasConversion<int>()
                    .IsRequired()
                    .HasDefaultValue(NotificationType.System)
                    .HasSentinel(NotificationType.System);
                entity
                    .Property(e => e.TargetType)
                    .HasColumnName("target_type")
                    .HasConversion<int>()
                    .IsRequired();
                entity.Property(e => e.Data).HasColumnName("data").HasColumnType("jsonb");

                entity
                    .HasMany(e => e.UserNotifications)
                    .WithOne(un => un.Notification)
                    .HasForeignKey(un => un.NotificationId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<UserNotification>(entity =>
            {
                entity.ConfigureBaseEntity();
                entity.ToTable("user_notifications");
                entity
                    .Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .HasColumnType("uuid")
                    .IsRequired();
                entity
                    .Property(e => e.NotificationId)
                    .HasColumnName("notification_id")
                    .HasColumnType("uuid")
                    .IsRequired();
                entity
                    .Property(e => e.IsRead)
                    .HasColumnName("is_read")
                    .IsRequired()
                    .HasDefaultValue(false);

                entity.HasIndex(e => new { e.UserId, e.NotificationId }).IsUnique();
            });
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Fatagram.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Fatagram.Infrastructure.Data.Extensions
{
    public static class NotificationExtension
    {
        public static void AddNotification(this ModelBuilder modelBuilder)
        {
            var dictionaryConverter = new ValueConverter<Dictionary<string, string>, string>(
                v => JsonSerializer.Serialize(v, default(JsonSerializerOptions)),
                v => JsonSerializer.Deserialize<Dictionary<string, string>>(v, default(JsonSerializerOptions)) ?? new Dictionary<string, string>()
            );

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(n => n.Data)
                      .HasConversion(dictionaryConverter)
                      .HasColumnType("jsonb");

                entity.HasOne<User>()
                      .WithMany()
                      .HasForeignKey(n => n.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
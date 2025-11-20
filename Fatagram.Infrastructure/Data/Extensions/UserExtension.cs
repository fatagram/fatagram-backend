using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fatagram.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Fatagram.Infrastructure.Data.Extensions
{
    public static class UserExtension
    {
        public static void AddUser(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.ConfigureBaseEntity();
                entity.ToTable("users");
                entity
                    .Property(e => e.LastName)
                    .HasColumnName("last_name")
                    .HasColumnType("VARCHAR(50)")
                    .IsRequired();
                entity
                    .Property(e => e.FirstName)
                    .HasColumnName("first_name")
                    .HasColumnType("VARCHAR(50)")
                    .IsRequired();
                entity
                    .Property(e => e.MiddleName)
                    .HasColumnName("middle_name")
                    .HasColumnType("VARCHAR(50)");
                entity
                    .Property(e => e.LastName)
                    .HasColumnName("last_name")
                    .HasColumnType("VARCHAR(50)")
                    .IsRequired();
                entity
                    .Property(e => e.FullName)
                    .HasColumnName("full_name")
                    .HasColumnType("VARCHAR(150)")
                    .IsRequired();
                entity.Property(e => e.BirthDay).HasColumnName("birth_day").HasColumnType("DATE");
                entity.Property(e => e.Phone).HasColumnName("phone").HasColumnType("VARCHAR(15)");
                entity.Property(e => e.Bio).HasColumnName("bio").HasColumnType("TEXT");
                entity.Property(e => e.Avatar).HasColumnName("avatar").HasColumnType("TEXT");
                entity
                    .Property(e => e.Background)
                    .HasColumnName("background")
                    .HasColumnType("TEXT");
                entity
                    .Property(e => e.LanguageCode)
                    .HasColumnName("language_code")
                    .HasColumnType("VARCHAR(10)")
                    .IsRequired();
                entity
                    .Property(e => e.LanguageCode)
                    .HasColumnName("language_code")
                    .HasColumnType("VARCHAR(10)")
                    .IsRequired();
                entity
                    .Property(e => e.UrlName)
                    .HasColumnName("url_name")
                    .HasColumnType("VARCHAR(30)");
                entity
                    .Property(e => e.Description)
                    .HasColumnName("description")
                    .HasColumnType("TEXT");
                entity
                    .Property(e => e.Gender)
                    .HasColumnName("gender")
                    .HasColumnType("SMALLINT")
                    .HasConversion<int>()
                    .IsRequired();

                // Relationships
                entity
                    .HasMany(a => a.Accounts)
                    .WithOne(u => u.User)
                    .HasForeignKey(a => a.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity
                    .HasOne(u => u.Language)
                    .WithMany(l => l.Users)
                    .HasForeignKey(u => u.LanguageCode)
                    .HasPrincipalKey(l => l.Code)
                    .OnDelete(DeleteBehavior.Restrict);

                entity
                    .HasMany(u => u.FriendRequests)
                    .WithOne(u => u.Sender)
                    .HasForeignKey(u => u.SenderId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity
                    .HasMany(u => u.FriendRequestsReceived)
                    .WithOne(u => u.Receiver)
                    .HasForeignKey(u => u.ReceiverId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity
                    .HasMany(u => u.FriendshipAsUser1)
                    .WithOne(u => u.User1)
                    .HasForeignKey(u => u.User1Id)
                    .OnDelete(DeleteBehavior.Restrict);

                entity
                    .HasMany(u => u.FriendshipAsUser2)
                    .WithOne(u => u.User2)
                    .HasForeignKey(u => u.User2Id)
                    .OnDelete(DeleteBehavior.Restrict);

                entity
                    .HasMany(e => e.UserNotifications)
                    .WithOne(n => n.User)
                    .HasForeignKey(n => n.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}

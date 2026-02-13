using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Fatagram.Infrastructure.Data.Extensions
{
    public static class EmailExtension
    {
        public static void AddEmail(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Email>(entity =>
            {
                entity.ConfigureBaseEntity();
                entity.ToTable("emails");
                entity
                    .Property(e => e.Address)
                    .HasColumnName("address")
                    .HasColumnType("VARCHAR(255)")
                    .IsRequired();
                entity.HasIndex(e => e.Address).IsUnique();
            });

            modelBuilder.Entity<UserEmail>(entity =>
            {
                entity.ConfigureBaseEntity();
                entity.ToTable("user_emails");
                entity
                    .Property(ue => ue.IsPrimary)
                    .HasColumnName("is_primary")
                    .HasColumnType("boolean")
                    .IsRequired();
                entity
                    .Property(ue => ue.IsVerified)
                    .HasColumnName("is_verified")
                    .HasColumnType("boolean")
                    .HasDefaultValue(false)
                    .IsRequired();
                entity
                    .Property(ue => ue.UserId)
                    .HasColumnName("user_id")
                    .HasColumnType("uuid")
                    .IsRequired();
                entity
                    .Property(ue => ue.EmailId)
                    .HasColumnName("email_id")
                    .HasColumnType("uuid")
                    .IsRequired();
                entity
                    .HasOne(ue => ue.User)
                    .WithMany(u => u.UserEmails)
                    .HasForeignKey(ue => ue.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity
                    .HasOne(ue => ue.Email)
                    .WithMany()
                    .HasForeignKey(ue => ue.EmailId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(ue => ue.UserId).HasFilter("is_primary = true").IsUnique();
                entity.HasIndex(ue => ue.EmailId).IsUnique();
            });
        }
    }
}

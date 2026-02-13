using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fatagram.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Fatagram.Infrastructure.Data.Extensions
{
    public static class AccountExtension
    {
        public static void AddAccount(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>(entity =>
            {
                entity.ConfigureBaseEntity();
                entity.ToTable("accounts");
                entity
                    .Property(a => a.Username)
                    .HasColumnName("username")
                    .HasColumnType("VARCHAR(30)");
                entity.Property(a => a.PasswordHash).HasColumnName("password_hash");
                entity.Property(a => a.UserId).HasColumnName("user_id").HasColumnType("uuid");
                entity
                    .Property(a => a.IsActive)
                    .HasColumnName("is_active")
                    .IsRequired()
                    .HasDefaultValue(true);
                // entity
                //     .Property(a => a.AuthProvider)
                //     .HasColumnName("auth_provider")
                //     .HasColumnType("VARCHAR(20)")
                //     .HasConversion<string>()
                //     .IsRequired();

                entity.HasIndex(a => a.Username).IsUnique();
            });
        }
    }
}

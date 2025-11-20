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
        public static void AddEmail(this ModelBuilder modelBuilder) =>
            modelBuilder.Entity<Email>(entity =>
            {
                entity.ConfigureBaseEntity();
                entity.ToTable("emails");
                entity
                    .Property(e => e.Address)
                    .HasColumnName("address")
                    .HasColumnType("VARCHAR(255)")
                    .IsRequired();
                entity
                    .Property(e => e.IsVerified)
                    .HasColumnName("is_verified")
                    .HasColumnType("BOOLEAN")
                    .IsRequired();
                entity.Property(e => e.AccountId).HasColumnName("account_id").IsRequired();
                entity
                    .Property(e => e.IsPrimary)
                    .HasColumnName("is_primary")
                    .IsRequired()
                    .HasDefaultValue(false);
                entity.HasIndex(e => e.Address).IsUnique();
            });
    }
}

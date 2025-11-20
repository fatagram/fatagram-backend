using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fatagram.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Fatagram.Infrastructure.Data.Extensions
{
    public static class RefreshTokenExtension
    {
        public static void AddRefreshToken(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.ConfigureBaseEntity();
                entity.ToTable("refresh_tokens");
                entity
                    .Property(rt => rt.Token)
                    .HasColumnName("token")
                    .HasColumnType("VARCHAR(200)")
                    .IsRequired();
                entity.Property(rt => rt.ExpiresAt).HasColumnName("expires_at").IsRequired();
                entity.HasIndex(rt => rt.Token).IsUnique();
            });
        }
    }
}

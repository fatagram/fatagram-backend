using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fatagram.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Fatagram.Infrastructure.Data.Extensions
{
    public static class LanguageExtension
    {
        public static void AddLanguage(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Language>(entity =>
            {
                entity.ConfigureBaseEntity();
                entity.ToTable("languages");
                entity
                    .Property(e => e.Code)
                    .HasColumnName("code")
                    .HasColumnType("VARCHAR(10)")
                    .IsRequired();
                entity
                    .Property(e => e.Name)
                    .HasColumnName("name")
                    .HasColumnType("VARCHAR(50)")
                    .IsRequired();

                // Code is used as principal key for relationships
                entity.HasAlternateKey(e => e.Code);
                entity.HasIndex(e => e.Code).IsUnique();
                // Seed data
                entity.HasData(
                    new Language()
                    {
                        Id = Guid.Parse("b3bb9f4e-1d6e-4f4a-9f7a-2c3b5e6d7f8a"),
                        Code = "en",
                        Name = "English",
                        CreatedAt = DateTime.UtcNow,
                    },
                    new Language()
                    {
                        Id = Guid.Parse("c4cc9f4e-2d7e-5f5a-0f8a-3d4c6f7e8f9b"),
                        Code = "vn",
                        Name = "Vietnamese",
                        CreatedAt = DateTime.UtcNow,
                    }
                );
            });
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Fatagram.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Fatagram.Infrastructure.Data.Extensions
{
    public static class LocalizedExtension
    {
        public static void AddLocalized(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Localized>(entity =>
            {
                entity.ConfigureBaseEntity();
                entity.ToTable("localizeds");
                entity
                    .Property(e => e.LanguageCode)
                    .HasColumnName("language_code")
                    .HasColumnType("VARCHAR(10)")
                    .IsRequired();
                entity
                    .Property(e => e.LocalizationKey)
                    .HasColumnName("localization_key")
                    .HasColumnType("VARCHAR(100)")
                    .IsRequired();
                entity
                    .Property(e => e.Value)
                    .HasColumnName("value")
                    .HasColumnType("TEXT")
                    .IsRequired();
            });
        }
    }
}

using System;
using Fatagram.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fatagram.Infrastructure.Data.Extensions
{
    public static class BaseEntityExtension
    {
        public static void ConfigureBaseEntity<TEntity>(this EntityTypeBuilder<TEntity> entity)
            where TEntity : BaseEntity
        {
            entity.HasKey(e => e.Id);
            entity
                .Property(e => e.Id)
                .HasColumnName("id")
                .HasColumnType("uuid")
                .HasDefaultValueSql("gen_random_uuid()");
            entity
                .Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");
            entity.Property(e => e.Version).IsRowVersion();
        }
    }
}

using Fatagram.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Fatagram.Infrastructure.Data.Extensions
{
    public static class RbacExtension
    {
        private static readonly Guid AdminRoleId = new("10000000-0000-0000-0000-000000000001");
        private static readonly Guid UserRoleId = new("10000000-0000-0000-0000-000000000002");
        private static readonly Guid ConversationMemberPermId = new(
            "20000000-0000-0000-0000-000000000001"
        );
        private static readonly Guid UserOwnerPermId = new("20000000-0000-0000-0000-000000000002");

        public static void AddRbac(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Permission>(entity =>
            {
                entity.ConfigureBaseEntity();
                entity.ToTable("permissions");
                entity
                    .Property(p => p.Name)
                    .HasColumnName("name")
                    .HasColumnType("VARCHAR(100)")
                    .IsRequired();
                entity
                    .Property(p => p.Description)
                    .HasColumnName("description")
                    .HasColumnType("text")
                    .IsRequired(false);
                entity.HasIndex(p => p.Name).IsUnique();
                entity
                    .HasMany(p => p.Roles)
                    .WithMany(r => r.Permissions)
                    .UsingEntity(j => j.ToTable("role_permissions"));
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ConfigureBaseEntity();
                entity.ToTable("roles");
                entity
                    .Property(r => r.Name)
                    .HasColumnName("name")
                    .HasColumnType("VARCHAR(100)")
                    .IsRequired();
                entity
                    .Property(r => r.Description)
                    .HasColumnName("description")
                    .HasColumnType("text")
                    .IsRequired(false);
                entity
                    .Property(r => r.IsSystem)
                    .HasColumnName("is_system")
                    .HasColumnType("boolean")
                    .HasDefaultValue(false)
                    .IsRequired();
                entity.HasIndex(r => r.Name).IsUnique();
            });

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.ConfigureBaseEntity();
                entity.ToTable("user_roles");
                entity
                    .Property(ur => ur.UserId)
                    .HasColumnName("user_id")
                    .HasColumnType("uuid")
                    .IsRequired();
                entity
                    .Property(ur => ur.RoleId)
                    .HasColumnName("role_id")
                    .HasColumnType("uuid")
                    .IsRequired();
                entity
                    .Property(ur => ur.ResourceId)
                    .HasColumnName("resource_id")
                    .HasColumnType("uuid")
                    .IsRequired(false);
                entity
                    .HasOne(ur => ur.User)
                    .WithMany()
                    .HasForeignKey(ur => ur.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity
                    .HasOne(ur => ur.Role)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.RoleId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity
                    .HasIndex(ur => new
                    {
                        ur.UserId,
                        ur.RoleId,
                        ur.ResourceId,
                    })
                    .IsUnique();
            });

            modelBuilder.Entity<RoutePermission>(entity =>
            {
                entity.ConfigureBaseEntity();
                entity.ToTable("route_permissions");
                entity
                    .Property(rp => rp.HttpMethod)
                    .HasColumnName("http_method")
                    .HasColumnType("VARCHAR(10)")
                    .HasDefaultValue("*")
                    .IsRequired();
                entity
                    .Property(rp => rp.RoutePattern)
                    .HasColumnName("route_pattern")
                    .HasColumnType("VARCHAR(500)")
                    .IsRequired();
                entity
                    .Property(rp => rp.PermissionName)
                    .HasColumnName("permission_name")
                    .HasColumnType("VARCHAR(100)")
                    .IsRequired();
                entity
                    .Property(rp => rp.ResourceParam)
                    .HasColumnName("resource_param")
                    .HasColumnType("VARCHAR(100)")
                    .IsRequired(false);
                entity
                    .Property(rp => rp.IsActive)
                    .HasColumnName("is_active")
                    .HasColumnType("boolean")
                    .HasDefaultValue(true)
                    .IsRequired();
                entity
                    .Property(rp => rp.Order)
                    .HasColumnName("order")
                    .HasColumnType("integer")
                    .HasDefaultValue(0)
                    .IsRequired();
                entity.HasIndex(rp => new { rp.HttpMethod, rp.RoutePattern }).IsUnique();
                entity.HasIndex(rp => rp.IsActive);
            });

            SeedData(modelBuilder);
        }

        private static void SeedData(ModelBuilder modelBuilder)
        {
            var now = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            modelBuilder
                .Entity<Role>()
                .HasData(
                    new Role
                    {
                        Id = AdminRoleId,
                        Name = "Admin",
                        Description = "System administrator",
                        IsSystem = true,
                        CreatedAt = now,
                    },
                    new Role
                    {
                        Id = UserRoleId,
                        Name = "User",
                        Description = "Regular user",
                        IsSystem = true,
                        CreatedAt = now,
                    }
                );

            modelBuilder
                .Entity<Permission>()
                .HasData(
                    new Permission
                    {
                        Id = ConversationMemberPermId,
                        Name = "conversation.member",
                        Description = "Access conversation resources as a member",
                        CreatedAt = now,
                    },
                    new Permission
                    {
                        Id = UserOwnerPermId,
                        Name = "user.owner",
                        Description = "Modify own user resources",
                        CreatedAt = now,
                    }
                );

            modelBuilder
                .Entity<RoutePermission>()
                .HasData(
                    new RoutePermission
                    {
                        Id = new Guid("30000000-0000-0000-0000-000000000001"),
                        HttpMethod = "*",
                        RoutePattern = "api/v1/conversations/{conversationId}/messages",
                        PermissionName = "conversation.member",
                        ResourceParam = "conversationId",
                        IsActive = true,
                        Order = 10,
                        CreatedAt = now,
                    },
                    new RoutePermission
                    {
                        Id = new Guid("30000000-0000-0000-0000-000000000002"),
                        HttpMethod = "*",
                        RoutePattern = "api/v1/conversations/{conversationId}/*",
                        PermissionName = "conversation.member",
                        ResourceParam = "conversationId",
                        IsActive = true,
                        Order = 20,
                        CreatedAt = now,
                    },
                    new RoutePermission
                    {
                        Id = new Guid("30000000-0000-0000-0000-000000000003"),
                        HttpMethod = "PUT",
                        RoutePattern = "api/v1/users/{userId}",
                        PermissionName = "user.owner",
                        ResourceParam = "userId",
                        IsActive = true,
                        Order = 10,
                        CreatedAt = now,
                    },
                    new RoutePermission
                    {
                        Id = new Guid("30000000-0000-0000-0000-000000000004"),
                        HttpMethod = "DELETE",
                        RoutePattern = "api/v1/users/{userId}",
                        PermissionName = "user.owner",
                        ResourceParam = "userId",
                        IsActive = true,
                        Order = 20,
                        CreatedAt = now,
                    }
                );
        }
    }
}

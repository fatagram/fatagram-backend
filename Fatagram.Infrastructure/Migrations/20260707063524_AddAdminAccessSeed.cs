using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fatagram.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAdminAccessSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "permissions",
                columns: new[] { "id", "created_at", "deleted_at", "description", "name", "updated_at" },
                values: new object[] { new Guid("20000000-0000-0000-0000-000000000003"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Access admin panel", "admin.access", null });

            migrationBuilder.InsertData(
                table: "route_permissions",
                columns: new[] { "id", "created_at", "deleted_at", "http_method", "is_active", "order", "permission_name", "resource_param", "route_pattern", "updated_at" },
                values: new object[] { new Guid("30000000-0000-0000-0000-000000000005"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "*", true, 1, "admin.access", null, "api/v1/admin/*", null });

            migrationBuilder.InsertData(
                table: "role_permissions",
                columns: new[] { "PermissionsId", "RolesId" },
                values: new object[] { new Guid("20000000-0000-0000-0000-000000000003"), new Guid("10000000-0000-0000-0000-000000000001") });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "PermissionsId", "RolesId" },
                keyValues: new object[] { new Guid("20000000-0000-0000-0000-000000000003"), new Guid("10000000-0000-0000-0000-000000000001") });

            migrationBuilder.DeleteData(
                table: "route_permissions",
                keyColumn: "id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000005"));

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000003"));
        }
    }
}

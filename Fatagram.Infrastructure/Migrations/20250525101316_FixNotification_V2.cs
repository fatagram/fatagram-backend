using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fatagram.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixNotification_V2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "actor_id",
                table: "Notifications",
                type: "uuid",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "actor_id",
                table: "Notifications");
        }
    }
}

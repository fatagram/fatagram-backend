using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fatagram.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSeenAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "is_on_boarding",
                table: "users",
                type: "BOOLEAN",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "BOOLEAN",
                oldDefaultValueSql: "FALSE");

            migrationBuilder.AddColumn<DateTime>(
                name: "seen_at",
                table: "conversation_participants",
                type: "timestamptz",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "seen_at",
                table: "conversation_participants");

            migrationBuilder.AlterColumn<bool>(
                name: "is_on_boarding",
                table: "users",
                type: "BOOLEAN",
                nullable: false,
                defaultValueSql: "FALSE",
                oldClrType: typeof(bool),
                oldType: "BOOLEAN",
                oldDefaultValue: false);
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fatagram.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPinnedAtToConversationParticipant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "pinned_at",
                table: "conversation_participants",
                type: "timestamptz",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "pinned_at",
                table: "conversation_participants");
        }
    }
}

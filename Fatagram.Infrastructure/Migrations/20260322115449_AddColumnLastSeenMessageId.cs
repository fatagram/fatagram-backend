using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fatagram.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddColumnLastSeenMessageId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "last_seen_message_id",
                table: "conversation_participants",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_conversation_participants_last_seen_message_id",
                table: "conversation_participants",
                column: "last_seen_message_id");

            migrationBuilder.AddForeignKey(
                name: "FK_conversation_participants_messages_last_seen_message_id",
                table: "conversation_participants",
                column: "last_seen_message_id",
                principalTable: "messages",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_conversation_participants_messages_last_seen_message_id",
                table: "conversation_participants");

            migrationBuilder.DropIndex(
                name: "IX_conversation_participants_last_seen_message_id",
                table: "conversation_participants");

            migrationBuilder.DropColumn(
                name: "last_seen_message_id",
                table: "conversation_participants");
        }
    }
}

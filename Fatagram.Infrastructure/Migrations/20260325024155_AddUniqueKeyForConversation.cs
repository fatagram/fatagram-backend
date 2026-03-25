using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fatagram.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueKeyForConversation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "unique_conversation_key",
                table: "conversations",
                type: "VARCHAR(255)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_conversations_unique_conversation_key",
                table: "conversations",
                column: "unique_conversation_key",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_conversations_unique_conversation_key",
                table: "conversations");

            migrationBuilder.DropColumn(
                name: "unique_conversation_key",
                table: "conversations");
        }
    }
}

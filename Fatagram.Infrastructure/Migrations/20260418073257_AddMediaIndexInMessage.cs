using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fatagram.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMediaIndexInMessage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "index_in_message",
                table: "message_media",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "message_sequence",
                table: "message_media",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "index_in_message",
                table: "message_media");

            migrationBuilder.DropColumn(
                name: "message_sequence",
                table: "message_media");
        }
    }
}

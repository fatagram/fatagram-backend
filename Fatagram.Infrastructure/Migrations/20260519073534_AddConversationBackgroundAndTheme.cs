using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fatagram.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddConversationBackgroundAndTheme : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "background_url",
                table: "conversations",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "theme",
                table: "conversations",
                type: "VARCHAR(255)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "background_url",
                table: "conversations");

            migrationBuilder.DropColumn(
                name: "theme",
                table: "conversations");
        }
    }
}

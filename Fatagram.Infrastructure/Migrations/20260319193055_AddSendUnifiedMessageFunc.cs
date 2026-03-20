using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fatagram.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSendUnifiedMessageFunc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var sqlPath = Path.Combine(AppContext.BaseDirectory, "Sql/SendMessage.sql");
            var sql = File.ReadAllText(sqlPath);
            migrationBuilder.Sql(sql);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "DROP FUNCTION IF EXISTS fn_send_unified_message(UUID, UUID, TEXT);"
            );
        }
    }
}

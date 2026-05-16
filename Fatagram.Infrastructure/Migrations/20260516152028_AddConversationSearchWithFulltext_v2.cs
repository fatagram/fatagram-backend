using Microsoft.EntityFrameworkCore.Migrations;
using NpgsqlTypes;

#nullable disable

namespace Fatagram.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddConversationSearchWithFulltext_v2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:pg_trgm", ",,")
                .Annotation("Npgsql:PostgresExtension:unaccent", ",,");

            migrationBuilder.AddColumn<string>(
                name: "search_text",
                table: "conversations",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<NpgsqlTsVector>(
                name: "search_vector",
                table: "conversations",
                type: "tsvector",
                nullable: true,
                computedColumnSql: "to_tsvector('simple', coalesce(search_text, ''))",
                stored: true);

            migrationBuilder.CreateIndex(
                name: "IX_conversations_search_text",
                table: "conversations",
                column: "search_text")
                .Annotation("Npgsql:IndexMethod", "GIN")
                .Annotation("Npgsql:IndexOperators", new[] { "gin_trgm_ops" });

            migrationBuilder.CreateIndex(
                name: "IX_conversations_search_vector",
                table: "conversations",
                column: "search_vector")
                .Annotation("Npgsql:IndexMethod", "GIN");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_conversations_search_text",
                table: "conversations");

            migrationBuilder.DropIndex(
                name: "IX_conversations_search_vector",
                table: "conversations");

            migrationBuilder.DropColumn(
                name: "search_vector",
                table: "conversations");

            migrationBuilder.DropColumn(
                name: "search_text",
                table: "conversations");

            migrationBuilder.AlterDatabase()
                .OldAnnotation("Npgsql:PostgresExtension:pg_trgm", ",,")
                .OldAnnotation("Npgsql:PostgresExtension:unaccent", ",,");
        }
    }
}

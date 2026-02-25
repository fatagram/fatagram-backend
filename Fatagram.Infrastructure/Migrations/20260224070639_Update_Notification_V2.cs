using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Fatagram.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Update_Notification_V2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "notification_contents");

            migrationBuilder.DropColumn(
                name: "target_id",
                table: "notifications");

            migrationBuilder.AlterColumn<Guid>(
                name: "actor_id",
                table: "notifications",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "actor_type",
                table: "notifications",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "actor_type",
                table: "notifications");

            migrationBuilder.AlterColumn<Guid>(
                name: "actor_id",
                table: "notifications",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "target_id",
                table: "notifications",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "notification_contents",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    language_code = table.Column<string>(type: "VARCHAR(2)", nullable: false),
                    content = table.Column<string>(type: "TEXT", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    type = table.Column<short>(type: "SMALLINT", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notification_contents", x => x.id);
                    table.ForeignKey(
                        name: "FK_notification_contents_languages_language_code",
                        column: x => x.language_code,
                        principalTable: "languages",
                        principalColumn: "code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "notification_contents",
                columns: new[] { "id", "content", "created_at", "deleted_at", "language_code", "type", "updated_at" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), "{actorName} sent you a friend request.", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "en", (short)0, null },
                    { new Guid("22222222-2222-2222-2222-222222222222"), "{actorName} accepted your friend request.", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "en", (short)1, null },
                    { new Guid("33333333-3333-3333-3333-333333333333"), "{actorName} đã gửi cho bạn một lời mời kết bạn.", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "vi", (short)0, null },
                    { new Guid("44444444-4444-4444-4444-444444444444"), "{actorName} đã chấp nhận lời mời kết bạn của bạn.", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "vi", (short)1, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_notification_contents_language_code",
                table: "notification_contents",
                column: "language_code");
        }
    }
}

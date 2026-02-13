using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fatagram.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Update_Email : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_emails_accounts_account_id",
                table: "emails");

            migrationBuilder.DropIndex(
                name: "IX_emails_account_id",
                table: "emails");

            migrationBuilder.DropColumn(
                name: "theme",
                table: "users");

            migrationBuilder.DropColumn(
                name: "account_id",
                table: "emails");

            migrationBuilder.DropColumn(
                name: "is_primary",
                table: "emails");

            migrationBuilder.DropColumn(
                name: "is_verified",
                table: "emails");

            migrationBuilder.CreateTable(
                name: "user_emails",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    email_id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_primary = table.Column<bool>(type: "boolean", nullable: false),
                    is_verified = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_emails", x => x.id);
                    table.ForeignKey(
                        name: "FK_user_emails_emails_email_id",
                        column: x => x.email_id,
                        principalTable: "emails",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_emails_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_user_emails_email_id",
                table: "user_emails",
                column: "email_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_emails_user_id",
                table: "user_emails",
                column: "user_id",
                unique: true,
                filter: "is_primary = true");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "user_emails");

            migrationBuilder.AddColumn<string>(
                name: "theme",
                table: "users",
                type: "VARCHAR(20)",
                nullable: false,
                defaultValue: "Neon");

            migrationBuilder.AddColumn<Guid>(
                name: "account_id",
                table: "emails",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<bool>(
                name: "is_primary",
                table: "emails",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "is_verified",
                table: "emails",
                type: "BOOLEAN",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_emails_account_id",
                table: "emails",
                column: "account_id");

            migrationBuilder.AddForeignKey(
                name: "FK_emails_accounts_account_id",
                table: "emails",
                column: "account_id",
                principalTable: "accounts",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

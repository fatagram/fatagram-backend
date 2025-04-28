using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fatagram.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFriend : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "friend_request",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    sender_id = table.Column<Guid>(type: "uuid", nullable: false),
                    receiver_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamptz", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_friend_request", x => x.id);
                    table.ForeignKey(
                        name: "FK_friend_request_users_receiver_id",
                        column: x => x.receiver_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_friend_request_users_sender_id",
                        column: x => x.sender_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "friendship",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_1_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_2_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamptz", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_friendship", x => x.id);
                    table.ForeignKey(
                        name: "FK_friendship_users_user_1_id",
                        column: x => x.user_1_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_friendship_users_user_2_id",
                        column: x => x.user_2_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_friend_request_receiver_id",
                table: "friend_request",
                column: "receiver_id");

            migrationBuilder.CreateIndex(
                name: "IX_friend_request_sender_id",
                table: "friend_request",
                column: "sender_id");

            migrationBuilder.CreateIndex(
                name: "IX_friendship_user_1_id",
                table: "friendship",
                column: "user_1_id");

            migrationBuilder.CreateIndex(
                name: "IX_friendship_user_2_id",
                table: "friendship",
                column: "user_2_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "friend_request");

            migrationBuilder.DropTable(
                name: "friendship");
        }
    }
}

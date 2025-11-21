using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Fatagram.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "languages",
                columns: table => new
                {
                    id = table.Column<Guid>(
                        type: "uuid",
                        nullable: false,
                        defaultValueSql: "gen_random_uuid()"
                    ),
                    code = table.Column<string>(type: "VARCHAR(10)", nullable: false),
                    name = table.Column<string>(type: "VARCHAR(50)", nullable: false),
                    created_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false,
                        defaultValueSql: "CURRENT_TIMESTAMP"
                    ),
                    updated_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    deleted_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_languages", x => x.id);
                    table.UniqueConstraint("AK_languages_code", x => x.code);
                }
            );

            migrationBuilder.CreateTable(
                name: "notifications",
                columns: table => new
                {
                    id = table.Column<Guid>(
                        type: "uuid",
                        nullable: false,
                        defaultValueSql: "gen_random_uuid()"
                    ),
                    actor_id = table.Column<Guid>(type: "uuid", nullable: true),
                    notification_type = table.Column<int>(
                        type: "integer",
                        nullable: false,
                        defaultValue: 3
                    ),
                    target_type = table.Column<int>(type: "integer", nullable: false),
                    target_id = table.Column<Guid>(type: "uuid", nullable: false),
                    data = table.Column<string>(type: "jsonb", nullable: true),
                    created_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false,
                        defaultValueSql: "CURRENT_TIMESTAMP"
                    ),
                    updated_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    deleted_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notifications", x => x.id);
                }
            );

            migrationBuilder.CreateTable(
                name: "localizeds",
                columns: table => new
                {
                    id = table.Column<Guid>(
                        type: "uuid",
                        nullable: false,
                        defaultValueSql: "gen_random_uuid()"
                    ),
                    localization_key = table.Column<string>(type: "VARCHAR(100)", nullable: false),
                    language_code = table.Column<string>(type: "VARCHAR(10)", nullable: false),
                    value = table.Column<string>(type: "TEXT", nullable: false),
                    LanguageId = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false,
                        defaultValueSql: "CURRENT_TIMESTAMP"
                    ),
                    updated_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    deleted_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_localizeds", x => x.id);
                    table.ForeignKey(
                        name: "FK_localizeds_languages_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "languages",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "notification_contents",
                columns: table => new
                {
                    id = table.Column<Guid>(
                        type: "uuid",
                        nullable: false,
                        defaultValueSql: "gen_random_uuid()"
                    ),
                    type = table.Column<short>(type: "SMALLINT", nullable: false),
                    language_code = table.Column<string>(type: "VARCHAR(2)", nullable: false),
                    content = table.Column<string>(type: "TEXT", nullable: false),
                    created_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false,
                        defaultValueSql: "CURRENT_TIMESTAMP"
                    ),
                    updated_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    deleted_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notification_contents", x => x.id);
                    table.ForeignKey(
                        name: "FK_notification_contents_languages_language_code",
                        column: x => x.language_code,
                        principalTable: "languages",
                        principalColumn: "code",
                        onDelete: ReferentialAction.Restrict
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<Guid>(
                        type: "uuid",
                        nullable: false,
                        defaultValueSql: "gen_random_uuid()"
                    ),
                    url_name = table.Column<string>(type: "VARCHAR(30)", nullable: true),
                    last_name = table.Column<string>(type: "VARCHAR(50)", nullable: true),
                    middle_name = table.Column<string>(type: "VARCHAR(50)", nullable: true),
                    first_name = table.Column<string>(type: "VARCHAR(50)", nullable: true),
                    full_name = table.Column<string>(type: "VARCHAR(150)", nullable: true),
                    phone = table.Column<string>(type: "VARCHAR(15)", nullable: true),
                    bio = table.Column<string>(type: "TEXT", nullable: true),
                    avatar = table.Column<string>(type: "TEXT", nullable: true),
                    background = table.Column<string>(type: "TEXT", nullable: true),
                    birth_day = table.Column<DateTime>(type: "DATE", nullable: true),
                    gender = table.Column<int>(type: "int", nullable: true),
                    language_code = table.Column<string>(
                        type: "VARCHAR(10)",
                        nullable: false,
                        defaultValue: "en"
                    ),
                    description = table.Column<string>(type: "TEXT", nullable: true),
                    nickname = table.Column<string>(type: "VARCHAR(50)", nullable: true),
                    is_on_boarding = table.Column<bool>(
                        type: "BOOLEAN",
                        nullable: false,
                        defaultValueSql: "FALSE"
                    ),
                    created_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false,
                        defaultValueSql: "CURRENT_TIMESTAMP"
                    ),
                    updated_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    deleted_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                    table.ForeignKey(
                        name: "FK_users_languages_language_code",
                        column: x => x.language_code,
                        principalTable: "languages",
                        principalColumn: "code",
                        onDelete: ReferentialAction.Restrict
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "accounts",
                columns: table => new
                {
                    id = table.Column<Guid>(
                        type: "uuid",
                        nullable: false,
                        defaultValueSql: "gen_random_uuid()"
                    ),
                    username = table.Column<string>(type: "VARCHAR(30)", nullable: false),
                    password_hash = table.Column<string>(type: "text", nullable: false),
                    is_active = table.Column<bool>(
                        type: "boolean",
                        nullable: false,
                        defaultValue: true
                    ),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false,
                        defaultValueSql: "CURRENT_TIMESTAMP"
                    ),
                    updated_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    deleted_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_accounts", x => x.id);
                    table.ForeignKey(
                        name: "FK_accounts_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "friend_requests",
                columns: table => new
                {
                    id = table.Column<Guid>(
                        type: "uuid",
                        nullable: false,
                        defaultValueSql: "gen_random_uuid()"
                    ),
                    sender_id = table.Column<Guid>(type: "uuid", nullable: false),
                    receiver_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false,
                        defaultValueSql: "CURRENT_TIMESTAMP"
                    ),
                    updated_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    deleted_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_friend_requests", x => x.id);
                    table.ForeignKey(
                        name: "FK_friend_requests_users_receiver_id",
                        column: x => x.receiver_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict
                    );
                    table.ForeignKey(
                        name: "FK_friend_requests_users_sender_id",
                        column: x => x.sender_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "friendships",
                columns: table => new
                {
                    id = table.Column<Guid>(
                        type: "uuid",
                        nullable: false,
                        defaultValueSql: "gen_random_uuid()"
                    ),
                    User1Id = table.Column<Guid>(type: "uuid", nullable: false),
                    User2Id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false,
                        defaultValueSql: "CURRENT_TIMESTAMP"
                    ),
                    updated_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    deleted_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_friendships", x => x.id);
                    table.ForeignKey(
                        name: "FK_friendships_users_User1Id",
                        column: x => x.User1Id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict
                    );
                    table.ForeignKey(
                        name: "FK_friendships_users_User2Id",
                        column: x => x.User2Id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "user_notifications",
                columns: table => new
                {
                    id = table.Column<Guid>(
                        type: "uuid",
                        nullable: false,
                        defaultValueSql: "gen_random_uuid()"
                    ),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    notification_id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_read = table.Column<bool>(
                        type: "boolean",
                        nullable: false,
                        defaultValue: false
                    ),
                    created_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false,
                        defaultValueSql: "CURRENT_TIMESTAMP"
                    ),
                    updated_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    deleted_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_notifications", x => x.id);
                    table.ForeignKey(
                        name: "FK_user_notifications_notifications_notification_id",
                        column: x => x.notification_id,
                        principalTable: "notifications",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_user_notifications_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "emails",
                columns: table => new
                {
                    id = table.Column<Guid>(
                        type: "uuid",
                        nullable: false,
                        defaultValueSql: "gen_random_uuid()"
                    ),
                    address = table.Column<string>(type: "VARCHAR(255)", nullable: false),
                    is_primary = table.Column<bool>(
                        type: "boolean",
                        nullable: false,
                        defaultValue: false
                    ),
                    is_verified = table.Column<bool>(type: "BOOLEAN", nullable: false),
                    account_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false,
                        defaultValueSql: "CURRENT_TIMESTAMP"
                    ),
                    updated_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    deleted_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_emails", x => x.id);
                    table.ForeignKey(
                        name: "FK_emails_accounts_account_id",
                        column: x => x.account_id,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "refresh_tokens",
                columns: table => new
                {
                    id = table.Column<Guid>(
                        type: "uuid",
                        nullable: false,
                        defaultValueSql: "gen_random_uuid()"
                    ),
                    token = table.Column<string>(type: "VARCHAR(200)", nullable: false),
                    AccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    expires_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    created_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false,
                        defaultValueSql: "CURRENT_TIMESTAMP"
                    ),
                    updated_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    deleted_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_refresh_tokens", x => x.id);
                    table.ForeignKey(
                        name: "FK_refresh_tokens_accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.InsertData(
                table: "languages",
                columns: new[] { "id", "code", "created_at", "deleted_at", "name", "updated_at" },
                values: new object[,]
                {
                    {
                        new Guid("b3bb9f4e-1d6e-4f4a-9f7a-2c3b5e6d7f8a"),
                        "en",
                        new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        null,
                        "English",
                        null,
                    },
                    {
                        new Guid("c4cc9f4e-2d7e-5f5a-0f8a-3d4c6f7e8f9b"),
                        "vi",
                        new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        null,
                        "Vietnamese",
                        null,
                    },
                }
            );

            migrationBuilder.InsertData(
                table: "notification_contents",
                columns: new[]
                {
                    "id",
                    "content",
                    "created_at",
                    "deleted_at",
                    "language_code",
                    "type",
                    "updated_at",
                },
                values: new object[,]
                {
                    {
                        new Guid("11111111-1111-1111-1111-111111111111"),
                        "{actorName} sent you a friend request.",
                        new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        null,
                        "en",
                        (short)0,
                        null,
                    },
                    {
                        new Guid("22222222-2222-2222-2222-222222222222"),
                        "{actorName} accepted your friend request.",
                        new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        null,
                        "en",
                        (short)1,
                        null,
                    },
                    {
                        new Guid("33333333-3333-3333-3333-333333333333"),
                        "{actorName} đã gửi cho bạn một lời mời kết bạn.",
                        new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        null,
                        "vi",
                        (short)0,
                        null,
                    },
                    {
                        new Guid("44444444-4444-4444-4444-444444444444"),
                        "{actorName} đã chấp nhận lời mời kết bạn của bạn.",
                        new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        null,
                        "vi",
                        (short)1,
                        null,
                    },
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_accounts_user_id",
                table: "accounts",
                column: "user_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_accounts_username",
                table: "accounts",
                column: "username",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_emails_account_id",
                table: "emails",
                column: "account_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_emails_address",
                table: "emails",
                column: "address",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_friend_requests_receiver_id",
                table: "friend_requests",
                column: "receiver_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_friend_requests_sender_id",
                table: "friend_requests",
                column: "sender_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_friendships_User1Id",
                table: "friendships",
                column: "User1Id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_friendships_User2Id",
                table: "friendships",
                column: "User2Id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_languages_code",
                table: "languages",
                column: "code",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_localizeds_LanguageId",
                table: "localizeds",
                column: "LanguageId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_notification_contents_language_code",
                table: "notification_contents",
                column: "language_code"
            );

            migrationBuilder.CreateIndex(
                name: "IX_refresh_tokens_AccountId",
                table: "refresh_tokens",
                column: "AccountId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_refresh_tokens_token",
                table: "refresh_tokens",
                column: "token",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_user_notifications_notification_id",
                table: "user_notifications",
                column: "notification_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_user_notifications_user_id_notification_id",
                table: "user_notifications",
                columns: new[] { "user_id", "notification_id" },
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_users_language_code",
                table: "users",
                column: "language_code"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "emails");

            migrationBuilder.DropTable(name: "friend_requests");

            migrationBuilder.DropTable(name: "friendships");

            migrationBuilder.DropTable(name: "localizeds");

            migrationBuilder.DropTable(name: "notification_contents");

            migrationBuilder.DropTable(name: "refresh_tokens");

            migrationBuilder.DropTable(name: "user_notifications");

            migrationBuilder.DropTable(name: "accounts");

            migrationBuilder.DropTable(name: "notifications");

            migrationBuilder.DropTable(name: "users");

            migrationBuilder.DropTable(name: "languages");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Fatagram.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "hobby",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    localization_key = table.Column<string>(type: "varchar(100)", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamptz", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_hobby", x => x.id);
                }
            );

            migrationBuilder.CreateTable(
                name: "job",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    localization_key = table.Column<string>(type: "varchar(100)", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamptz", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_job", x => x.id);
                }
            );

            migrationBuilder.CreateTable(
                name: "Languages",
                columns: table => new
                {
                    code = table.Column<string>(type: "varchar(2)", nullable: false),
                    name = table.Column<string>(type: "varchar(50)", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Languages", x => x.code);
                }
            );

            migrationBuilder.CreateTable(
                name: "school",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    localization_key = table.Column<string>(type: "varchar(100)", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamptz", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_school", x => x.id);
                }
            );

            migrationBuilder.CreateTable(
                name: "skill",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    localization_key = table.Column<string>(type: "varchar(100)", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamptz", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_skill", x => x.id);
                }
            );

            migrationBuilder.CreateTable(
                name: "localized",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    localization_key = table.Column<string>(type: "varchar(100)", nullable: false),
                    language_code = table.Column<string>(type: "varchar(2)", nullable: false),
                    value = table.Column<string>(type: "text", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_localized", x => x.id);
                    table.ForeignKey(
                        name: "FK_localized_Languages_language_code",
                        column: x => x.language_code,
                        principalTable: "Languages",
                        principalColumn: "code",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "NotificationContents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    lang_code = table.Column<string>(type: "varchar(2)", nullable: false),
                    content = table.Column<string>(type: "text", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationContents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotificationContents_Languages_lang_code",
                        column: x => x.lang_code,
                        principalTable: "Languages",
                        principalColumn: "code",
                        onDelete: ReferentialAction.Restrict
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    urlname = table.Column<string>(type: "varchar(50)", nullable: true),
                    last_name = table.Column<string>(type: "varchar(50)", nullable: false),
                    first_name = table.Column<string>(type: "varchar(50)", nullable: false),
                    full_name = table.Column<string>(type: "varchar(100)", nullable: false),
                    email = table.Column<string>(type: "varchar(100)", nullable: false),
                    phone = table.Column<string>(type: "varchar(10)", nullable: true),
                    bio = table.Column<string>(type: "text", nullable: true),
                    avatar = table.Column<string>(type: "text", nullable: true),
                    background = table.Column<string>(type: "text", nullable: true),
                    birth_day = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    gender = table.Column<string>(type: "varchar(10)", nullable: true),
                    lang_code = table.Column<string>(type: "varchar(2)", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    nickname = table.Column<string>(type: "varchar(50)", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                    table.ForeignKey(
                        name: "FK_users_Languages_lang_code",
                        column: x => x.lang_code,
                        principalTable: "Languages",
                        principalColumn: "code",
                        onDelete: ReferentialAction.Restrict
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "accounts",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    username = table.Column<string>(type: "varchar(50)", nullable: false),
                    password_hash = table.Column<string>(type: "varchar(100)", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
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
                name: "friend_request",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    sender_id = table.Column<Guid>(type: "uuid", nullable: false),
                    receiver_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamptz", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_friend_request", x => x.id);
                    table.ForeignKey(
                        name: "FK_friend_request_users_receiver_id",
                        column: x => x.receiver_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict
                    );
                    table.ForeignKey(
                        name: "FK_friend_request_users_sender_id",
                        column: x => x.sender_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "friendship",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_1_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_2_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamptz", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_friendship", x => x.id);
                    table.ForeignKey(
                        name: "FK_friendship_users_user_1_id",
                        column: x => x.user_1_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict
                    );
                    table.ForeignKey(
                        name: "FK_friendship_users_user_2_id",
                        column: x => x.user_2_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "HobbyUser",
                columns: table => new
                {
                    HobbiesId = table.Column<Guid>(type: "uuid", nullable: false),
                    UsersId = table.Column<Guid>(type: "uuid", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HobbyUser", x => new { x.HobbiesId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_HobbyUser_hobby_HobbiesId",
                        column: x => x.HobbiesId,
                        principalTable: "hobby",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_HobbyUser_users_UsersId",
                        column: x => x.UsersId,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    data = table.Column<string>(type: "jsonb", nullable: false),
                    type = table.Column<short>(type: "smallint", nullable: false),
                    actor_id = table.Column<Guid>(type: "uuid", nullable: true),
                    link = table.Column<string>(type: "text", nullable: false),
                    is_read = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamptz", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.id);
                    table.ForeignKey(
                        name: "FK_Notifications_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "SkillUser",
                columns: table => new
                {
                    SkillsId = table.Column<Guid>(type: "uuid", nullable: false),
                    UsersId = table.Column<Guid>(type: "uuid", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkillUser", x => new { x.SkillsId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_SkillUser_skill_SkillsId",
                        column: x => x.SkillsId,
                        principalTable: "skill",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_SkillUser_users_UsersId",
                        column: x => x.UsersId,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "user_job",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    job_id = table.Column<Guid>(type: "uuid", nullable: false),
                    start = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    end = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    job_state = table.Column<string>(type: "varchar(20)", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_job", x => x.id);
                    table.ForeignKey(
                        name: "FK_user_job_job_job_id",
                        column: x => x.job_id,
                        principalTable: "job",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_user_job_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "user_privacies",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    field = table.Column<string>(type: "varchar(10)", nullable: false),
                    level = table.Column<string>(type: "varchar(10)", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_privacies", x => x.id);
                    table.ForeignKey(
                        name: "FK_user_privacies_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "user_school",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    school_id = table.Column<Guid>(type: "uuid", nullable: false),
                    start = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    end = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    school_state = table.Column<string>(type: "varchar(20)", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_school", x => x.id);
                    table.ForeignKey(
                        name: "FK_user_school_school_school_id",
                        column: x => x.school_id,
                        principalTable: "school",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_user_school_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "refresh_tokens",
                columns: table => new
                {
                    token = table.Column<Guid>(type: "uuid", nullable: false),
                    account_id = table.Column<Guid>(type: "uuid", nullable: false),
                    expiry_date = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamptz", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_refresh_tokens", x => x.token);
                    table.ForeignKey(
                        name: "FK_refresh_tokens_accounts_account_id",
                        column: x => x.account_id,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.InsertData(
                table: "Languages",
                columns: new[] { "code", "name" },
                values: new object[,]
                {
                    { "en", "English" },
                    { "vi", "Tiếng Việt" },
                }
            );

            migrationBuilder.InsertData(
                table: "hobby",
                columns: new[] { "id", "created_at", "localization_key", "updated_at" },
                values: new object[,]
                {
                    {
                        new Guid("11111111-1111-1111-1111-111111111111"),
                        new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                        "HOBBY_MUSIC",
                        new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                    },
                    {
                        new Guid("22222222-2222-2222-2222-222222222222"),
                        new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                        "HOBBY_SPORT",
                        new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                    },
                    {
                        new Guid("33333333-3333-3333-3333-333333333333"),
                        new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                        "HOBBY_TRAVEL",
                        new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                    },
                    {
                        new Guid("44444444-4444-4444-4444-444444444444"),
                        new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                        "HOBBY_READING",
                        new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                    },
                }
            );

            migrationBuilder.InsertData(
                table: "skill",
                columns: new[] { "id", "created_at", "localization_key", "updated_at" },
                values: new object[,]
                {
                    {
                        new Guid("55555555-5555-5555-5555-555555555555"),
                        new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                        "SKILL_PROGRAMMING",
                        new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                    },
                    {
                        new Guid("66666666-6666-6666-6666-666666666666"),
                        new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                        "SKILL_DESIGN",
                        new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                    },
                    {
                        new Guid("77777777-7777-7777-7777-777777777777"),
                        new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                        "SKILL_MANAGEMENT",
                        new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                    },
                    {
                        new Guid("88888888-8888-8888-8888-888888888888"),
                        new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                        "SKILL_MARKETING",
                        new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                    },
                }
            );

            migrationBuilder.InsertData(
                table: "NotificationContents",
                columns: new[] { "Id", "content", "lang_code", "Type" },
                values: new object[,]
                {
                    {
                        new Guid("11111111-1111-1111-1111-111111111111"),
                        "{actorName} sent you a friend request.",
                        "en",
                        0,
                    },
                    {
                        new Guid("22222222-2222-2222-2222-222222222222"),
                        "{actorName} accepted your friend request.",
                        "en",
                        1,
                    },
                    {
                        new Guid("33333333-3333-3333-3333-333333333333"),
                        "{actorName} đã gửi cho bạn một lời mời kết bạn.",
                        "vi",
                        0,
                    },
                    {
                        new Guid("44444444-4444-4444-4444-444444444444"),
                        "{actorName} đã chấp nhận lời mời kết bạn của bạn.",
                        "vi",
                        1,
                    },
                }
            );

            migrationBuilder.InsertData(
                table: "localized",
                columns: new[] { "id", "language_code", "localization_key", "value" },
                values: new object[,]
                {
                    {
                        new Guid("00000000-0000-0000-0000-000000000001"),
                        "vi",
                        "SKILL_MANAGEMENT",
                        "Quản lý",
                    },
                    {
                        new Guid("00000000-0000-0000-0000-000000000002"),
                        "vi",
                        "SKILL_MARKETING",
                        "Tiếp thị",
                    },
                    {
                        new Guid("11111111-1111-1111-1111-111111111111"),
                        "en",
                        "HOBBY_MUSIC",
                        "Music",
                    },
                    {
                        new Guid("22222222-2222-2222-2222-222222222222"),
                        "en",
                        "HOBBY_SPORTS",
                        "Sports",
                    },
                    {
                        new Guid("33333333-3333-3333-3333-333333333333"),
                        "en",
                        "HOBBY_TRAVEL",
                        "Travel",
                    },
                    {
                        new Guid("44444444-4444-4444-4444-444444444444"),
                        "en",
                        "HOBBY_READING",
                        "Reading",
                    },
                    {
                        new Guid("55555555-5555-5555-5555-555555555555"),
                        "vi",
                        "HOBBY_MUSIC",
                        "Âm nhạc",
                    },
                    {
                        new Guid("66666666-6666-6666-6666-666666666666"),
                        "vi",
                        "HOBBY_SPORTS",
                        "Thể thao",
                    },
                    {
                        new Guid("77777777-7777-7777-7777-777777777777"),
                        "vi",
                        "HOBBY_TRAVEL",
                        "Du lịch",
                    },
                    {
                        new Guid("88888888-8888-8888-8888-888888888888"),
                        "vi",
                        "HOBBY_READING",
                        "Đọc sách",
                    },
                    {
                        new Guid("99999999-9999-9999-9999-999999999999"),
                        "en",
                        "SKILL_PROGRAMMING",
                        "Programming",
                    },
                    {
                        new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                        "en",
                        "SKILL_DESIGN",
                        "Design",
                    },
                    {
                        new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                        "en",
                        "SKILL_MANAGEMENT",
                        "Management",
                    },
                    {
                        new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                        "en",
                        "SKILL_MARKETING",
                        "Marketing",
                    },
                    {
                        new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"),
                        "vi",
                        "SKILL_PROGRAMMING",
                        "Lập trình",
                    },
                    {
                        new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"),
                        "vi",
                        "SKILL_DESIGN",
                        "Thiết kế",
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
                name: "IX_friend_request_receiver_id",
                table: "friend_request",
                column: "receiver_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_friend_request_sender_id",
                table: "friend_request",
                column: "sender_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_friendship_user_1_id",
                table: "friendship",
                column: "user_1_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_friendship_user_2_id",
                table: "friendship",
                column: "user_2_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_HobbyUser_UsersId",
                table: "HobbyUser",
                column: "UsersId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_localized_language_code",
                table: "localized",
                column: "language_code"
            );

            migrationBuilder.CreateIndex(
                name: "IX_NotificationContents_lang_code",
                table: "NotificationContents",
                column: "lang_code"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_user_id",
                table: "Notifications",
                column: "user_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_refresh_tokens_account_id",
                table: "refresh_tokens",
                column: "account_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_SkillUser_UsersId",
                table: "SkillUser",
                column: "UsersId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_user_job_job_id",
                table: "user_job",
                column: "job_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_user_job_user_id",
                table: "user_job",
                column: "user_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_user_privacies_user_id",
                table: "user_privacies",
                column: "user_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_user_school_school_id",
                table: "user_school",
                column: "school_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_user_school_user_id",
                table: "user_school",
                column: "user_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_users_email",
                table: "users",
                column: "email",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_users_lang_code",
                table: "users",
                column: "lang_code"
            );

            migrationBuilder.CreateIndex(
                name: "IX_users_urlname",
                table: "users",
                column: "urlname",
                unique: true
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "friend_request");

            migrationBuilder.DropTable(name: "friendship");

            migrationBuilder.DropTable(name: "HobbyUser");

            migrationBuilder.DropTable(name: "localized");

            migrationBuilder.DropTable(name: "NotificationContents");

            migrationBuilder.DropTable(name: "Notifications");

            migrationBuilder.DropTable(name: "refresh_tokens");

            migrationBuilder.DropTable(name: "SkillUser");

            migrationBuilder.DropTable(name: "user_job");

            migrationBuilder.DropTable(name: "user_privacies");

            migrationBuilder.DropTable(name: "user_school");

            migrationBuilder.DropTable(name: "hobby");

            migrationBuilder.DropTable(name: "accounts");

            migrationBuilder.DropTable(name: "skill");

            migrationBuilder.DropTable(name: "job");

            migrationBuilder.DropTable(name: "school");

            migrationBuilder.DropTable(name: "users");

            migrationBuilder.DropTable(name: "Languages");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Fatagram.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddChatThemeTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "role",
                table: "conversation_participants",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 2);

            migrationBuilder.CreateTable(
                name: "chat_themes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    key = table.Column<string>(type: "VARCHAR(100)", nullable: false),
                    label = table.Column<string>(type: "VARCHAR(200)", nullable: false),
                    category = table.Column<string>(type: "VARCHAR(100)", nullable: false, defaultValue: "General"),
                    is_default = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    is_event = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    bg_image = table.Column<string>(type: "text", nullable: true),
                    light_colors_json = table.Column<string>(type: "text", nullable: true),
                    dark_colors_json = table.Column<string>(type: "text", nullable: true),
                    start_date = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    end_date = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    sort_order = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_chat_themes", x => x.id);
                });

            migrationBuilder.InsertData(
                table: "chat_themes",
                columns: new[] { "id", "bg_image", "category", "created_at", "dark_colors_json", "deleted_at", "end_date", "is_active", "is_default", "key", "label", "light_colors_json", "start_date", "updated_at" },
                values: new object[] { new Guid("40000000-0000-0000-0000-000000000001"), null, "General", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, true, true, "default", "Mặc định", null, null, null });

            migrationBuilder.InsertData(
                table: "chat_themes",
                columns: new[] { "id", "bg_image", "category", "created_at", "dark_colors_json", "deleted_at", "end_date", "is_active", "key", "label", "light_colors_json", "sort_order", "start_date", "updated_at" },
                values: new object[,]
                {
                    { new Guid("40000000-0000-0000-0000-000000000002"), null, "General", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "{\"gradient\":\"bg-gradient-to-tr from-[#0f766e] to-[#34d399]\",\"primaryLight\":\"#34d399\",\"primaryMain\":\"#10b981\",\"bgMain\":\"#0a1812\",\"bgSecond\":\"#0f2019\"}", null, null, true, "chat-emerald", "Emerald", "{\"gradient\":\"bg-gradient-to-tr from-[#107a51] to-[#85e3ad]\",\"primaryLight\":\"#85e3ad\",\"primaryMain\":\"#107a51\",\"bgMain\":\"#f0f8f5\",\"bgSecond\":\"#d7f0e4\"}", 1, null, null },
                    { new Guid("40000000-0000-0000-0000-000000000003"), null, "General", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "{\"gradient\":\"bg-gradient-to-tr from-[#9a3412] to-[#f97316]\",\"primaryLight\":\"#f97316\",\"primaryMain\":\"#ea580c\",\"bgMain\":\"#18100c\",\"bgSecond\":\"#201611\"}", null, null, true, "chat-sunset", "Sunset", "{\"gradient\":\"bg-gradient-to-tr from-[#f26c4f] to-[#ffd700]\",\"primaryLight\":\"#ffd700\",\"primaryMain\":\"#f26c4f\",\"bgMain\":\"#fff8f3\",\"bgSecond\":\"#ffe4d2\"}", 2, null, null },
                    { new Guid("40000000-0000-0000-0000-000000000004"), "/images/cyberpunk_bg.png", "Special", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "{\"gradient\":\"bg-gradient-to-tr from-[#ff007f] to-[#00f2fe]\",\"primaryLight\":\"#00f2fe\",\"primaryMain\":\"#ff007f\",\"bgMain\":\"#12101a\",\"bgSecond\":\"#191624\"}", null, null, true, "chat-cyberpunk", "Cyberpunk", "{\"gradient\":\"bg-gradient-to-tr from-[#db2777] to-[#06b6d4]\",\"primaryLight\":\"#06b6d4\",\"primaryMain\":\"#db2777\",\"bgMain\":\"#fdf4ff\",\"bgSecond\":\"#f5e5fa\"}", 3, null, null },
                    { new Guid("40000000-0000-0000-0000-000000000005"), null, "General", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "{\"gradient\":\"bg-gradient-to-tr from-[#6d28d9] to-[#c4b5fd]\",\"primaryLight\":\"#c4b5fd\",\"primaryMain\":\"#8b5cf6\",\"bgMain\":\"#0f0d16\",\"bgSecond\":\"#15121e\"}", null, null, true, "chat-lavender", "Lavender", "{\"gradient\":\"bg-gradient-to-tr from-[#7255de] to-[#c4b5fd]\",\"primaryLight\":\"#c4b5fd\",\"primaryMain\":\"#7255de\",\"bgMain\":\"#f8f6fe\",\"bgSecond\":\"#e6e0f8\"}", 4, null, null },
                    { new Guid("40000000-0000-0000-0000-000000000006"), null, "General", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "{\"gradient\":\"bg-gradient-to-tr from-[#0369a1] to-[#38bdf8]\",\"primaryLight\":\"#38bdf8\",\"primaryMain\":\"#0284c7\",\"bgMain\":\"#08121a\",\"bgSecond\":\"#0c1a26\"}", null, null, true, "chat-ocean", "Ocean Deep", "{\"gradient\":\"bg-gradient-to-tr from-[#0e74b5] to-[#90e0ef]\",\"primaryLight\":\"#90e0ef\",\"primaryMain\":\"#0e74b5\",\"bgMain\":\"#f2f8fc\",\"bgSecond\":\"#d7ebf8\"}", 5, null, null },
                    { new Guid("40000000-0000-0000-0000-000000000007"), null, "General", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "{\"gradient\":\"bg-gradient-to-tr from-[#be185d] to-[#f472b6]\",\"primaryLight\":\"#f472b6\",\"primaryMain\":\"#db2777\",\"bgMain\":\"#180c12\",\"bgSecond\":\"#221018\"}", null, null, true, "chat-bubblegum", "Bubblegum", "{\"gradient\":\"bg-gradient-to-tr from-[#ec4887] to-[#fbcfe8]\",\"primaryLight\":\"#fbcfe8\",\"primaryMain\":\"#ec4887\",\"bgMain\":\"#fef6f8\",\"bgSecond\":\"#f9dce6\"}", 6, null, null }
                });

            migrationBuilder.InsertData(
                table: "chat_themes",
                columns: new[] { "id", "bg_image", "category", "created_at", "dark_colors_json", "deleted_at", "end_date", "is_active", "is_event", "key", "label", "light_colors_json", "sort_order", "start_date", "updated_at" },
                values: new object[] { new Guid("40000000-0000-0000-0000-000000000008"), "/images/worldcup_bg.png", "Event", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "{\"gradient\":\"bg-gradient-to-tr from-[#166534] to-[#22c55e]\",\"primaryLight\":\"#22c55e\",\"primaryMain\":\"#15803d\",\"bgMain\":\"#0a1812\",\"bgSecond\":\"#0f2019\"}", null, null, true, true, "chat-worldcup", "World Cup", "{\"gradient\":\"bg-gradient-to-tr from-[#15803d] to-[#4ade80]\",\"primaryLight\":\"#4ade80\",\"primaryMain\":\"#15803d\",\"bgMain\":\"#f0f8f5\",\"bgSecond\":\"#d7f0e4\"}", 7, null, null });

            migrationBuilder.InsertData(
                table: "chat_themes",
                columns: new[] { "id", "bg_image", "category", "created_at", "dark_colors_json", "deleted_at", "end_date", "is_active", "key", "label", "light_colors_json", "sort_order", "start_date", "updated_at" },
                values: new object[,]
                {
                    { new Guid("40000000-0000-0000-0000-000000000009"), "/images/vietnam_bg.png", "Special", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "{\"gradient\":\"bg-gradient-to-tr from-[#991b1b] to-[#ffcd00]\",\"primaryLight\":\"#ffcd00\",\"primaryMain\":\"#da251d\",\"bgMain\":\"#180a0a\",\"bgSecond\":\"#240f0f\"}", null, null, true, "chat-vietnam", "Việt Nam", "{\"gradient\":\"bg-gradient-to-tr from-[#da251d] to-[#ffcd00]\",\"primaryLight\":\"#ffcd00\",\"primaryMain\":\"#da251d\",\"bgMain\":\"#fef2f2\",\"bgSecond\":\"#fee2e2\"}", 8, null, null },
                    { new Guid("40000000-0000-0000-0000-000000000010"), "/images/vutru_bg.png", "Special", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "{\"gradient\":\"bg-gradient-to-tr from-[#312e81] to-[#831843]\",\"primaryLight\":\"#831843\",\"primaryMain\":\"#4c1d95\",\"bgMain\":\"#0a0718\",\"bgSecond\":\"#100c24\"}", null, null, true, "chat-vutru", "Vũ trụ", "{\"gradient\":\"bg-gradient-to-tr from-[#6366f1] to-[#ec4899]\",\"primaryLight\":\"#ec4899\",\"primaryMain\":\"#8b5cf6\",\"bgMain\":\"#f5f3ff\",\"bgSecond\":\"#ede9fe\"}", 9, null, null }
                });

            migrationBuilder.InsertData(
                table: "chat_themes",
                columns: new[] { "id", "bg_image", "category", "created_at", "dark_colors_json", "deleted_at", "end_date", "is_active", "is_event", "key", "label", "light_colors_json", "sort_order", "start_date", "updated_at" },
                values: new object[,]
                {
                    { new Guid("40000000-0000-0000-0000-000000000011"), "/images/halloween_bg.png", "Seasonal", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "{\"gradient\":\"bg-gradient-to-tr from-[#3b0764] to-[#f97316]\",\"primaryLight\":\"#f97316\",\"primaryMain\":\"#a855f7\",\"bgMain\":\"#090212\",\"bgSecond\":\"#140526\"}", null, null, true, true, "chat-halloween", "Halloween", "{\"gradient\":\"bg-gradient-to-tr from-[#7c2d12] to-[#c2410c]\",\"primaryLight\":\"#f97316\",\"primaryMain\":\"#c2410c\",\"bgMain\":\"#fff7ed\",\"bgSecond\":\"#ffedd5\"}", 10, null, null },
                    { new Guid("40000000-0000-0000-0000-000000000012"), "/images/christmas_bg.png", "Seasonal", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "{\"gradient\":\"bg-gradient-to-tr from-[#14532d] to-[#b91c1c]\",\"primaryLight\":\"#b91c1c\",\"primaryMain\":\"#16a34a\",\"bgMain\":\"#180404\",\"bgSecond\":\"#260808\"}", null, null, true, true, "chat-christmas", "Giáng sinh", "{\"gradient\":\"bg-gradient-to-tr from-[#991b1b] to-[#f59e0b]\",\"primaryLight\":\"#f59e0b\",\"primaryMain\":\"#b91c1c\",\"bgMain\":\"#fef2f2\",\"bgSecond\":\"#fee2e2\"}", 11, null, null },
                    { new Guid("40000000-0000-0000-0000-000000000013"), "/images/summer_bg.png", "Seasonal", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "{\"gradient\":\"bg-gradient-to-tr from-[#0369a1] to-[#ca8a04]\",\"primaryLight\":\"#facc15\",\"primaryMain\":\"#0284c7\",\"bgMain\":\"#06101e\",\"bgSecond\":\"#0a192e\"}", null, null, true, true, "chat-summer", "Mùa hè", "{\"gradient\":\"bg-gradient-to-tr from-[#0284c7] to-[#f59e0b]\",\"primaryLight\":\"#38bdf8\",\"primaryMain\":\"#f59e0b\",\"bgMain\":\"#f0f9ff\",\"bgSecond\":\"#e0f2fe\"}", 12, null, null },
                    { new Guid("40000000-0000-0000-0000-000000000014"), "/images/cr7_bg.png", "Event", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "{\"gradient\":\"bg-gradient-to-tr from-[#052e16] to-[#eab308]\",\"primaryLight\":\"#eab308\",\"primaryMain\":\"#16a34a\",\"bgMain\":\"#021008\",\"bgSecond\":\"#052010\"}", null, null, true, true, "chat-cr7", "CR7", "{\"gradient\":\"bg-gradient-to-tr from-[#15803d] to-[#facc15]\",\"primaryLight\":\"#facc15\",\"primaryMain\":\"#15803d\",\"bgMain\":\"#f0fdf4\",\"bgSecond\":\"#dcfce7\"}", 13, null, null }
                });

            migrationBuilder.InsertData(
                table: "chat_themes",
                columns: new[] { "id", "bg_image", "category", "created_at", "dark_colors_json", "deleted_at", "end_date", "is_active", "key", "label", "light_colors_json", "sort_order", "start_date", "updated_at" },
                values: new object[,]
                {
                    { new Guid("40000000-0000-0000-0000-000000000015"), null, "General", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "{\"gradient\":\"bg-gradient-to-tr from-[#1e053a] to-[#db2777]\",\"primaryLight\":\"#db2777\",\"primaryMain\":\"#8b5cf6\",\"bgMain\":\"#0d0515\",\"bgSecond\":\"#160a22\"}", null, null, true, "chat-midnight", "Midnight Blossom", "{\"gradient\":\"bg-gradient-to-tr from-[#2e0854] to-[#f472b6]\",\"primaryLight\":\"#f472b6\",\"primaryMain\":\"#2e0854\",\"bgMain\":\"#faf5ff\",\"bgSecond\":\"#f3e8ff\"}", 14, null, null },
                    { new Guid("40000000-0000-0000-0000-000000000016"), null, "Seasonal", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "{\"gradient\":\"bg-gradient-to-tr from-[#78350f] to-[#f59e0b]\",\"primaryLight\":\"#f59e0b\",\"primaryMain\":\"#d97706\",\"bgMain\":\"#170f0a\",\"bgSecond\":\"#22160f\"}", null, null, true, "chat-autumn", "Golden Autumn", "{\"gradient\":\"bg-gradient-to-tr from-[#b45309] to-[#fcd34d]\",\"primaryLight\":\"#fcd34d\",\"primaryMain\":\"#b45309\",\"bgMain\":\"#fffbeb\",\"bgSecond\":\"#fef3c7\"}", 15, null, null },
                    { new Guid("40000000-0000-0000-0000-000000000017"), null, "General", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "{\"gradient\":\"bg-gradient-to-tr from-[#075985] to-[#38bdf8]\",\"primaryLight\":\"#38bdf8\",\"primaryMain\":\"#0284c7\",\"bgMain\":\"#08131a\",\"bgSecond\":\"#0c1d29\"}", null, null, true, "chat-glacier", "Frosty Glacier", "{\"gradient\":\"bg-gradient-to-tr from-[#0369a1] to-[#e0f2fe]\",\"primaryLight\":\"#38bdf8\",\"primaryMain\":\"#0369a1\",\"bgMain\":\"#f0f9ff\",\"bgSecond\":\"#e0f2fe\"}", 16, null, null },
                    { new Guid("40000000-0000-0000-0000-000000000018"), null, "General", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "{\"gradient\":\"bg-gradient-to-tr from-[#020617] to-[#22c55e]\",\"primaryLight\":\"#22c55e\",\"primaryMain\":\"#10b981\",\"bgMain\":\"#050814\",\"bgSecond\":\"#0b1021\"}", null, null, true, "chat-neon", "Neon Oasis", "{\"gradient\":\"bg-gradient-to-tr from-[#0f172a] to-[#10b981]\",\"primaryLight\":\"#10b981\",\"primaryMain\":\"#0f172a\",\"bgMain\":\"#f8fafc\",\"bgSecond\":\"#f1f5f9\"}", 17, null, null },
                    { new Guid("40000000-0000-0000-0000-000000000019"), null, "General", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "{\"gradient\":\"bg-gradient-to-tr from-[#9f1239] to-[#fb7185]\",\"primaryLight\":\"#fb7185\",\"primaryMain\":\"#e11d48\",\"bgMain\":\"#180b0e\",\"bgSecond\":\"#241014\"}", null, null, true, "chat-rose", "Rose Quartz", "{\"gradient\":\"bg-gradient-to-tr from-[#be123c] to-[#ffe4e6]\",\"primaryLight\":\"#fb7185\",\"primaryMain\":\"#be123c\",\"bgMain\":\"#fff1f2\",\"bgSecond\":\"#ffe4e6\"}", 18, null, null },
                    { new Guid("40000000-0000-0000-0000-000000000020"), null, "General", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "{\"gradient\":\"bg-gradient-to-tr from-[#14532d] to-[#4ade80]\",\"primaryLight\":\"#4ade80\",\"primaryMain\":\"#16a34a\",\"bgMain\":\"#09140e\",\"bgSecond\":\"#0e1f15\"}", null, null, true, "chat-forest", "Forest Mist", "{\"gradient\":\"bg-gradient-to-tr from-[#166534] to-[#dcfce7]\",\"primaryLight\":\"#4ade80\",\"primaryMain\":\"#166534\",\"bgMain\":\"#f0fdf4\",\"bgSecond\":\"#dcfce7\"}", 19, null, null },
                    { new Guid("40000000-0000-0000-0000-000000000021"), null, "General", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "{\"gradient\":\"bg-gradient-to-tr from-[#172554] to-[#eab308]\",\"primaryLight\":\"#eab308\",\"primaryMain\":\"#ca8a04\",\"bgMain\":\"#060b18\",\"bgSecond\":\"#0a1226\"}", null, null, true, "chat-royal", "Royal Amber", "{\"gradient\":\"bg-gradient-to-tr from-[#1e3a8a] to-[#fef08a]\",\"primaryLight\":\"#facc15\",\"primaryMain\":\"#1e3a8a\",\"bgMain\":\"#eff6ff\",\"bgSecond\":\"#dbeafe\"}", 20, null, null },
                    { new Guid("40000000-0000-0000-0000-000000000022"), null, "General", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "{\"gradient\":\"bg-gradient-to-tr from-[#312e81] to-[#ec4899]\",\"primaryLight\":\"#ec4899\",\"primaryMain\":\"#d946ef\",\"bgMain\":\"#090816\",\"bgSecond\":\"#0e0d24\"}", null, null, true, "chat-tokyo", "Tokyo Drift", "{\"gradient\":\"bg-gradient-to-tr from-[#4f46e5] to-[#f472b6]\",\"primaryLight\":\"#f472b6\",\"primaryMain\":\"#4f46e5\",\"bgMain\":\"#eef2ff\",\"bgSecond\":\"#e0e7ff\"}", 21, null, null },
                    { new Guid("40000000-0000-0000-0000-000000000023"), null, "General", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "{\"gradient\":\"bg-gradient-to-tr from-[#365314] to-[#a3e635]\",\"primaryLight\":\"#a3e635\",\"primaryMain\":\"#84cc16\",\"bgMain\":\"#0e1507\",\"bgSecond\":\"#16220b\"}", null, null, true, "chat-matcha", "Matcha Latte", "{\"gradient\":\"bg-gradient-to-tr from-[#3f6212] to-[#d9f99d]\",\"primaryLight\":\"#a3e635\",\"primaryMain\":\"#3f6212\",\"bgMain\":\"#f7fee7\",\"bgSecond\":\"#ecfccb\"}", 22, null, null },
                    { new Guid("40000000-0000-0000-0000-000000000024"), null, "General", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "{\"gradient\":\"bg-gradient-to-tr from-[#9d174d] to-[#f472b6]\",\"primaryLight\":\"#f472b6\",\"primaryMain\":\"#db2777\",\"bgMain\":\"#1c0d14\",\"bgSecond\":\"#2a131e\"}", null, null, true, "chat-sakura", "Sakura Cherry", "{\"gradient\":\"bg-gradient-to-tr from-[#db2777] to-[#fdf2f8]\",\"primaryLight\":\"#f472b6\",\"primaryMain\":\"#db2777\",\"bgMain\":\"#fff5f7\",\"bgSecond\":\"#ffe4e6\"}", 23, null, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_chat_themes_is_active",
                table: "chat_themes",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "IX_chat_themes_key",
                table: "chat_themes",
                column: "key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_chat_themes_sort_order",
                table: "chat_themes",
                column: "sort_order");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "chat_themes");

            migrationBuilder.AlterColumn<int>(
                name: "role",
                table: "conversation_participants",
                type: "integer",
                nullable: false,
                defaultValue: 2,
                oldClrType: typeof(int),
                oldType: "integer");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text.Json;
using Fatagram.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Fatagram.Infrastructure.Data.Extensions
{
    public static class ChatThemeExtension
    {
        public static void AddChatTheme(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ChatTheme>(entity =>
            {
                entity.ConfigureBaseEntity();
                entity.ToTable("chat_themes");

                entity
                    .Property(ct => ct.Key)
                    .HasColumnName("key")
                    .HasColumnType("VARCHAR(100)")
                    .IsRequired();

                entity
                    .Property(ct => ct.Label)
                    .HasColumnName("label")
                    .HasColumnType("VARCHAR(200)")
                    .IsRequired();

                entity
                    .Property(ct => ct.Category)
                    .HasColumnName("category")
                    .HasColumnType("VARCHAR(100)")
                    .HasDefaultValue("General")
                    .IsRequired();

                entity
                    .Property(ct => ct.IsDefault)
                    .HasColumnName("is_default")
                    .HasColumnType("boolean")
                    .HasDefaultValue(false)
                    .IsRequired();

                entity
                    .Property(ct => ct.IsActive)
                    .HasColumnName("is_active")
                    .HasColumnType("boolean")
                    .HasDefaultValue(true)
                    .IsRequired();

                entity
                    .Property(ct => ct.IsEvent)
                    .HasColumnName("is_event")
                    .HasColumnType("boolean")
                    .HasDefaultValue(false)
                    .IsRequired();

                entity
                    .Property(ct => ct.BgImage)
                    .HasColumnName("bg_image")
                    .HasColumnType("text")
                    .IsRequired(false);

                entity
                    .Property(ct => ct.LightColorsJson)
                    .HasColumnName("light_colors_json")
                    .HasColumnType("text")
                    .IsRequired(false);

                entity
                    .Property(ct => ct.DarkColorsJson)
                    .HasColumnName("dark_colors_json")
                    .HasColumnType("text")
                    .IsRequired(false);

                entity
                    .Property(ct => ct.StartDate)
                    .HasColumnName("start_date")
                    .HasColumnType("timestamptz")
                    .IsRequired(false);

                entity
                    .Property(ct => ct.EndDate)
                    .HasColumnName("end_date")
                    .HasColumnType("timestamptz")
                    .IsRequired(false);

                entity
                    .Property(ct => ct.SortOrder)
                    .HasColumnName("sort_order")
                    .HasColumnType("integer")
                    .HasDefaultValue(0)
                    .IsRequired();

                entity.HasIndex(ct => ct.Key).IsUnique();
                entity.HasIndex(ct => ct.IsActive);
                entity.HasIndex(ct => ct.SortOrder);
            });

            SeedInitialThemes(modelBuilder);
        }

        private static void SeedInitialThemes(ModelBuilder modelBuilder)
        {
            var now = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            var themes = new List<ChatTheme>
            {
                new()
                {
                    Id = new Guid("40000000-0000-0000-0000-000000000001"),
                    Key = "default",
                    Label = "Mặc định",
                    Category = "General",
                    IsDefault = true,
                    IsActive = true,
                    IsEvent = false,
                    SortOrder = 0,
                    CreatedAt = now,
                },
                new()
                {
                    Id = new Guid("40000000-0000-0000-0000-000000000002"),
                    Key = "chat-emerald",
                    Label = "Emerald",
                    Category = "General",
                    IsDefault = false,
                    IsActive = true,
                    IsEvent = false,
                    LightColorsJson = JsonSerializer.Serialize(new
                    {
                        gradient = "bg-gradient-to-tr from-[#107a51] to-[#85e3ad]",
                        primaryLight = "#85e3ad",
                        primaryMain = "#107a51",
                        bgMain = "#f0f8f5",
                        bgSecond = "#d7f0e4"
                    }),
                    DarkColorsJson = JsonSerializer.Serialize(new
                    {
                        gradient = "bg-gradient-to-tr from-[#0f766e] to-[#34d399]",
                        primaryLight = "#34d399",
                        primaryMain = "#10b981",
                        bgMain = "#0a1812",
                        bgSecond = "#0f2019"
                    }),
                    SortOrder = 1,
                    CreatedAt = now,
                },
                new()
                {
                    Id = new Guid("40000000-0000-0000-0000-000000000003"),
                    Key = "chat-sunset",
                    Label = "Sunset",
                    Category = "General",
                    IsDefault = false,
                    IsActive = true,
                    IsEvent = false,
                    LightColorsJson = JsonSerializer.Serialize(new
                    {
                        gradient = "bg-gradient-to-tr from-[#f26c4f] to-[#ffd700]",
                        primaryLight = "#ffd700",
                        primaryMain = "#f26c4f",
                        bgMain = "#fff8f3",
                        bgSecond = "#ffe4d2"
                    }),
                    DarkColorsJson = JsonSerializer.Serialize(new
                    {
                        gradient = "bg-gradient-to-tr from-[#9a3412] to-[#f97316]",
                        primaryLight = "#f97316",
                        primaryMain = "#ea580c",
                        bgMain = "#18100c",
                        bgSecond = "#201611"
                    }),
                    SortOrder = 2,
                    CreatedAt = now,
                },
                new()
                {
                    Id = new Guid("40000000-0000-0000-0000-000000000004"),
                    Key = "chat-cyberpunk",
                    Label = "Cyberpunk",
                    Category = "Special",
                    IsDefault = false,
                    IsActive = true,
                    IsEvent = false,
                    BgImage = "/images/cyberpunk_bg.png",
                    LightColorsJson = JsonSerializer.Serialize(new
                    {
                        gradient = "bg-gradient-to-tr from-[#db2777] to-[#06b6d4]",
                        primaryLight = "#06b6d4",
                        primaryMain = "#db2777",
                        bgMain = "#fdf4ff",
                        bgSecond = "#f5e5fa"
                    }),
                    DarkColorsJson = JsonSerializer.Serialize(new
                    {
                        gradient = "bg-gradient-to-tr from-[#ff007f] to-[#00f2fe]",
                        primaryLight = "#00f2fe",
                        primaryMain = "#ff007f",
                        bgMain = "#12101a",
                        bgSecond = "#191624"
                    }),
                    SortOrder = 3,
                    CreatedAt = now,
                },
                new()
                {
                    Id = new Guid("40000000-0000-0000-0000-000000000005"),
                    Key = "chat-lavender",
                    Label = "Lavender",
                    Category = "General",
                    IsDefault = false,
                    IsActive = true,
                    IsEvent = false,
                    LightColorsJson = JsonSerializer.Serialize(new
                    {
                        gradient = "bg-gradient-to-tr from-[#7255de] to-[#c4b5fd]",
                        primaryLight = "#c4b5fd",
                        primaryMain = "#7255de",
                        bgMain = "#f8f6fe",
                        bgSecond = "#e6e0f8"
                    }),
                    DarkColorsJson = JsonSerializer.Serialize(new
                    {
                        gradient = "bg-gradient-to-tr from-[#6d28d9] to-[#c4b5fd]",
                        primaryLight = "#c4b5fd",
                        primaryMain = "#8b5cf6",
                        bgMain = "#0f0d16",
                        bgSecond = "#15121e"
                    }),
                    SortOrder = 4,
                    CreatedAt = now,
                },
                new()
                {
                    Id = new Guid("40000000-0000-0000-0000-000000000006"),
                    Key = "chat-ocean",
                    Label = "Ocean Deep",
                    Category = "General",
                    IsDefault = false,
                    IsActive = true,
                    IsEvent = false,
                    LightColorsJson = JsonSerializer.Serialize(new
                    {
                        gradient = "bg-gradient-to-tr from-[#0e74b5] to-[#90e0ef]",
                        primaryLight = "#90e0ef",
                        primaryMain = "#0e74b5",
                        bgMain = "#f2f8fc",
                        bgSecond = "#d7ebf8"
                    }),
                    DarkColorsJson = JsonSerializer.Serialize(new
                    {
                        gradient = "bg-gradient-to-tr from-[#0369a1] to-[#38bdf8]",
                        primaryLight = "#38bdf8",
                        primaryMain = "#0284c7",
                        bgMain = "#08121a",
                        bgSecond = "#0c1a26"
                    }),
                    SortOrder = 5,
                    CreatedAt = now,
                },
                new()
                {
                    Id = new Guid("40000000-0000-0000-0000-000000000007"),
                    Key = "chat-bubblegum",
                    Label = "Bubblegum",
                    Category = "General",
                    IsDefault = false,
                    IsActive = true,
                    IsEvent = false,
                    LightColorsJson = JsonSerializer.Serialize(new
                    {
                        gradient = "bg-gradient-to-tr from-[#ec4887] to-[#fbcfe8]",
                        primaryLight = "#fbcfe8",
                        primaryMain = "#ec4887",
                        bgMain = "#fef6f8",
                        bgSecond = "#f9dce6"
                    }),
                    DarkColorsJson = JsonSerializer.Serialize(new
                    {
                        gradient = "bg-gradient-to-tr from-[#be185d] to-[#f472b6]",
                        primaryLight = "#f472b6",
                        primaryMain = "#db2777",
                        bgMain = "#180c12",
                        bgSecond = "#221018"
                    }),
                    SortOrder = 6,
                    CreatedAt = now,
                },
                new()
                {
                    Id = new Guid("40000000-0000-0000-0000-000000000008"),
                    Key = "chat-worldcup",
                    Label = "World Cup",
                    Category = "Event",
                    IsDefault = false,
                    IsActive = true,
                    IsEvent = true,
                    BgImage = "/images/worldcup_bg.png",
                    LightColorsJson = JsonSerializer.Serialize(new
                    {
                        gradient = "bg-gradient-to-tr from-[#15803d] to-[#4ade80]",
                        primaryLight = "#4ade80",
                        primaryMain = "#15803d",
                        bgMain = "#f0f8f5",
                        bgSecond = "#d7f0e4"
                    }),
                    DarkColorsJson = JsonSerializer.Serialize(new
                    {
                        gradient = "bg-gradient-to-tr from-[#166534] to-[#22c55e]",
                        primaryLight = "#22c55e",
                        primaryMain = "#15803d",
                        bgMain = "#0a1812",
                        bgSecond = "#0f2019"
                    }),
                    SortOrder = 7,
                    CreatedAt = now,
                },
                new()
                {
                    Id = new Guid("40000000-0000-0000-0000-000000000009"),
                    Key = "chat-vietnam",
                    Label = "Việt Nam",
                    Category = "Special",
                    IsDefault = false,
                    IsActive = true,
                    IsEvent = false,
                    BgImage = "/images/vietnam_bg.png",
                    LightColorsJson = JsonSerializer.Serialize(new
                    {
                        gradient = "bg-gradient-to-tr from-[#da251d] to-[#ffcd00]",
                        primaryLight = "#ffcd00",
                        primaryMain = "#da251d",
                        bgMain = "#fef2f2",
                        bgSecond = "#fee2e2"
                    }),
                    DarkColorsJson = JsonSerializer.Serialize(new
                    {
                        gradient = "bg-gradient-to-tr from-[#991b1b] to-[#ffcd00]",
                        primaryLight = "#ffcd00",
                        primaryMain = "#da251d",
                        bgMain = "#180a0a",
                        bgSecond = "#240f0f"
                    }),
                    SortOrder = 8,
                    CreatedAt = now,
                },
                new()
                {
                    Id = new Guid("40000000-0000-0000-0000-000000000010"),
                    Key = "chat-vutru",
                    Label = "Vũ trụ",
                    Category = "Special",
                    IsDefault = false,
                    IsActive = true,
                    IsEvent = false,
                    BgImage = "/images/vutru_bg.png",
                    LightColorsJson = JsonSerializer.Serialize(new
                    {
                        gradient = "bg-gradient-to-tr from-[#6366f1] to-[#ec4899]",
                        primaryLight = "#ec4899",
                        primaryMain = "#8b5cf6",
                        bgMain = "#f5f3ff",
                        bgSecond = "#ede9fe"
                    }),
                    DarkColorsJson = JsonSerializer.Serialize(new
                    {
                        gradient = "bg-gradient-to-tr from-[#312e81] to-[#831843]",
                        primaryLight = "#831843",
                        primaryMain = "#4c1d95",
                        bgMain = "#0a0718",
                        bgSecond = "#100c24"
                    }),
                    SortOrder = 9,
                    CreatedAt = now,
                },
                new()
                {
                    Id = new Guid("40000000-0000-0000-0000-000000000011"),
                    Key = "chat-halloween",
                    Label = "Halloween",
                    Category = "Seasonal",
                    IsDefault = false,
                    IsActive = true,
                    IsEvent = true,
                    BgImage = "/images/halloween_bg.png",
                    LightColorsJson = JsonSerializer.Serialize(new
                    {
                        gradient = "bg-gradient-to-tr from-[#7c2d12] to-[#c2410c]",
                        primaryLight = "#f97316",
                        primaryMain = "#c2410c",
                        bgMain = "#fff7ed",
                        bgSecond = "#ffedd5"
                    }),
                    DarkColorsJson = JsonSerializer.Serialize(new
                    {
                        gradient = "bg-gradient-to-tr from-[#3b0764] to-[#f97316]",
                        primaryLight = "#f97316",
                        primaryMain = "#a855f7",
                        bgMain = "#090212",
                        bgSecond = "#140526"
                    }),
                    SortOrder = 10,
                    CreatedAt = now,
                },
                new()
                {
                    Id = new Guid("40000000-0000-0000-0000-000000000012"),
                    Key = "chat-christmas",
                    Label = "Giáng sinh",
                    Category = "Seasonal",
                    IsDefault = false,
                    IsActive = true,
                    IsEvent = true,
                    BgImage = "/images/christmas_bg.png",
                    LightColorsJson = JsonSerializer.Serialize(new
                    {
                        gradient = "bg-gradient-to-tr from-[#991b1b] to-[#f59e0b]",
                        primaryLight = "#f59e0b",
                        primaryMain = "#b91c1c",
                        bgMain = "#fef2f2",
                        bgSecond = "#fee2e2"
                    }),
                    DarkColorsJson = JsonSerializer.Serialize(new
                    {
                        gradient = "bg-gradient-to-tr from-[#14532d] to-[#b91c1c]",
                        primaryLight = "#b91c1c",
                        primaryMain = "#16a34a",
                        bgMain = "#180404",
                        bgSecond = "#260808"
                    }),
                    SortOrder = 11,
                    CreatedAt = now,
                },
                new()
                {
                    Id = new Guid("40000000-0000-0000-0000-000000000013"),
                    Key = "chat-summer",
                    Label = "Mùa hè",
                    Category = "Seasonal",
                    IsDefault = false,
                    IsActive = true,
                    IsEvent = true,
                    BgImage = "/images/summer_bg.png",
                    LightColorsJson = JsonSerializer.Serialize(new
                    {
                        gradient = "bg-gradient-to-tr from-[#0284c7] to-[#f59e0b]",
                        primaryLight = "#38bdf8",
                        primaryMain = "#f59e0b",
                        bgMain = "#f0f9ff",
                        bgSecond = "#e0f2fe"
                    }),
                    DarkColorsJson = JsonSerializer.Serialize(new
                    {
                        gradient = "bg-gradient-to-tr from-[#0369a1] to-[#ca8a04]",
                        primaryLight = "#facc15",
                        primaryMain = "#0284c7",
                        bgMain = "#06101e",
                        bgSecond = "#0a192e"
                    }),
                    SortOrder = 12,
                    CreatedAt = now,
                },
                new()
                {
                    Id = new Guid("40000000-0000-0000-0000-000000000014"),
                    Key = "chat-cr7",
                    Label = "CR7",
                    Category = "Event",
                    IsDefault = false,
                    IsActive = true,
                    IsEvent = true,
                    BgImage = "/images/cr7_bg.png",
                    LightColorsJson = JsonSerializer.Serialize(new
                    {
                        gradient = "bg-gradient-to-tr from-[#15803d] to-[#facc15]",
                        primaryLight = "#facc15",
                        primaryMain = "#15803d",
                        bgMain = "#f0fdf4",
                        bgSecond = "#dcfce7"
                    }),
                    DarkColorsJson = JsonSerializer.Serialize(new
                    {
                        gradient = "bg-gradient-to-tr from-[#052e16] to-[#eab308]",
                        primaryLight = "#eab308",
                        primaryMain = "#16a34a",
                        bgMain = "#021008",
                        bgSecond = "#052010"
                    }),
                    SortOrder = 13,
                    CreatedAt = now,
                },
                new()
                {
                    Id = new Guid("40000000-0000-0000-0000-000000000015"),
                    Key = "chat-midnight",
                    Label = "Midnight Blossom",
                    Category = "General",
                    IsDefault = false,
                    IsActive = true,
                    IsEvent = false,
                    LightColorsJson = JsonSerializer.Serialize(new
                    {
                        gradient = "bg-gradient-to-tr from-[#2e0854] to-[#f472b6]",
                        primaryLight = "#f472b6",
                        primaryMain = "#2e0854",
                        bgMain = "#faf5ff",
                        bgSecond = "#f3e8ff"
                    }),
                    DarkColorsJson = JsonSerializer.Serialize(new
                    {
                        gradient = "bg-gradient-to-tr from-[#1e053a] to-[#db2777]",
                        primaryLight = "#db2777",
                        primaryMain = "#8b5cf6",
                        bgMain = "#0d0515",
                        bgSecond = "#160a22"
                    }),
                    SortOrder = 14,
                    CreatedAt = now,
                },
                new()
                {
                    Id = new Guid("40000000-0000-0000-0000-000000000016"),
                    Key = "chat-autumn",
                    Label = "Golden Autumn",
                    Category = "Seasonal",
                    IsDefault = false,
                    IsActive = true,
                    IsEvent = false,
                    LightColorsJson = JsonSerializer.Serialize(new
                    {
                        gradient = "bg-gradient-to-tr from-[#b45309] to-[#fcd34d]",
                        primaryLight = "#fcd34d",
                        primaryMain = "#b45309",
                        bgMain = "#fffbeb",
                        bgSecond = "#fef3c7"
                    }),
                    DarkColorsJson = JsonSerializer.Serialize(new
                    {
                        gradient = "bg-gradient-to-tr from-[#78350f] to-[#f59e0b]",
                        primaryLight = "#f59e0b",
                        primaryMain = "#d97706",
                        bgMain = "#170f0a",
                        bgSecond = "#22160f"
                    }),
                    SortOrder = 15,
                    CreatedAt = now,
                },
                new()
                {
                    Id = new Guid("40000000-0000-0000-0000-000000000017"),
                    Key = "chat-glacier",
                    Label = "Frosty Glacier",
                    Category = "General",
                    IsDefault = false,
                    IsActive = true,
                    IsEvent = false,
                    LightColorsJson = JsonSerializer.Serialize(new
                    {
                        gradient = "bg-gradient-to-tr from-[#0369a1] to-[#e0f2fe]",
                        primaryLight = "#38bdf8",
                        primaryMain = "#0369a1",
                        bgMain = "#f0f9ff",
                        bgSecond = "#e0f2fe"
                    }),
                    DarkColorsJson = JsonSerializer.Serialize(new
                    {
                        gradient = "bg-gradient-to-tr from-[#075985] to-[#38bdf8]",
                        primaryLight = "#38bdf8",
                        primaryMain = "#0284c7",
                        bgMain = "#08131a",
                        bgSecond = "#0c1d29"
                    }),
                    SortOrder = 16,
                    CreatedAt = now,
                },
                new()
                {
                    Id = new Guid("40000000-0000-0000-0000-000000000018"),
                    Key = "chat-neon",
                    Label = "Neon Oasis",
                    Category = "General",
                    IsDefault = false,
                    IsActive = true,
                    IsEvent = false,
                    LightColorsJson = JsonSerializer.Serialize(new
                    {
                        gradient = "bg-gradient-to-tr from-[#0f172a] to-[#10b981]",
                        primaryLight = "#10b981",
                        primaryMain = "#0f172a",
                        bgMain = "#f8fafc",
                        bgSecond = "#f1f5f9"
                    }),
                    DarkColorsJson = JsonSerializer.Serialize(new
                    {
                        gradient = "bg-gradient-to-tr from-[#020617] to-[#22c55e]",
                        primaryLight = "#22c55e",
                        primaryMain = "#10b981",
                        bgMain = "#050814",
                        bgSecond = "#0b1021"
                    }),
                    SortOrder = 17,
                    CreatedAt = now,
                },
                new()
                {
                    Id = new Guid("40000000-0000-0000-0000-000000000019"),
                    Key = "chat-rose",
                    Label = "Rose Quartz",
                    Category = "General",
                    IsDefault = false,
                    IsActive = true,
                    IsEvent = false,
                    LightColorsJson = JsonSerializer.Serialize(new
                    {
                        gradient = "bg-gradient-to-tr from-[#be123c] to-[#ffe4e6]",
                        primaryLight = "#fb7185",
                        primaryMain = "#be123c",
                        bgMain = "#fff1f2",
                        bgSecond = "#ffe4e6"
                    }),
                    DarkColorsJson = JsonSerializer.Serialize(new
                    {
                        gradient = "bg-gradient-to-tr from-[#9f1239] to-[#fb7185]",
                        primaryLight = "#fb7185",
                        primaryMain = "#e11d48",
                        bgMain = "#180b0e",
                        bgSecond = "#241014"
                    }),
                    SortOrder = 18,
                    CreatedAt = now,
                },
                new()
                {
                    Id = new Guid("40000000-0000-0000-0000-000000000020"),
                    Key = "chat-forest",
                    Label = "Forest Mist",
                    Category = "General",
                    IsDefault = false,
                    IsActive = true,
                    IsEvent = false,
                    LightColorsJson = JsonSerializer.Serialize(new
                    {
                        gradient = "bg-gradient-to-tr from-[#166534] to-[#dcfce7]",
                        primaryLight = "#4ade80",
                        primaryMain = "#166534",
                        bgMain = "#f0fdf4",
                        bgSecond = "#dcfce7"
                    }),
                    DarkColorsJson = JsonSerializer.Serialize(new
                    {
                        gradient = "bg-gradient-to-tr from-[#14532d] to-[#4ade80]",
                        primaryLight = "#4ade80",
                        primaryMain = "#16a34a",
                        bgMain = "#09140e",
                        bgSecond = "#0e1f15"
                    }),
                    SortOrder = 19,
                    CreatedAt = now,
                },
                new()
                {
                    Id = new Guid("40000000-0000-0000-0000-000000000021"),
                    Key = "chat-royal",
                    Label = "Royal Amber",
                    Category = "General",
                    IsDefault = false,
                    IsActive = true,
                    IsEvent = false,
                    LightColorsJson = JsonSerializer.Serialize(new
                    {
                        gradient = "bg-gradient-to-tr from-[#1e3a8a] to-[#fef08a]",
                        primaryLight = "#facc15",
                        primaryMain = "#1e3a8a",
                        bgMain = "#eff6ff",
                        bgSecond = "#dbeafe"
                    }),
                    DarkColorsJson = JsonSerializer.Serialize(new
                    {
                        gradient = "bg-gradient-to-tr from-[#172554] to-[#eab308]",
                        primaryLight = "#eab308",
                        primaryMain = "#ca8a04",
                        bgMain = "#060b18",
                        bgSecond = "#0a1226"
                    }),
                    SortOrder = 20,
                    CreatedAt = now,
                },
                new()
                {
                    Id = new Guid("40000000-0000-0000-0000-000000000022"),
                    Key = "chat-tokyo",
                    Label = "Tokyo Drift",
                    Category = "General",
                    IsDefault = false,
                    IsActive = true,
                    IsEvent = false,
                    LightColorsJson = JsonSerializer.Serialize(new
                    {
                        gradient = "bg-gradient-to-tr from-[#4f46e5] to-[#f472b6]",
                        primaryLight = "#f472b6",
                        primaryMain = "#4f46e5",
                        bgMain = "#eef2ff",
                        bgSecond = "#e0e7ff"
                    }),
                    DarkColorsJson = JsonSerializer.Serialize(new
                    {
                        gradient = "bg-gradient-to-tr from-[#312e81] to-[#ec4899]",
                        primaryLight = "#ec4899",
                        primaryMain = "#d946ef",
                        bgMain = "#090816",
                        bgSecond = "#0e0d24"
                    }),
                    SortOrder = 21,
                    CreatedAt = now,
                },
                new()
                {
                    Id = new Guid("40000000-0000-0000-0000-000000000023"),
                    Key = "chat-matcha",
                    Label = "Matcha Latte",
                    Category = "General",
                    IsDefault = false,
                    IsActive = true,
                    IsEvent = false,
                    LightColorsJson = JsonSerializer.Serialize(new
                    {
                        gradient = "bg-gradient-to-tr from-[#3f6212] to-[#d9f99d]",
                        primaryLight = "#a3e635",
                        primaryMain = "#3f6212",
                        bgMain = "#f7fee7",
                        bgSecond = "#ecfccb"
                    }),
                    DarkColorsJson = JsonSerializer.Serialize(new
                    {
                        gradient = "bg-gradient-to-tr from-[#365314] to-[#a3e635]",
                        primaryLight = "#a3e635",
                        primaryMain = "#84cc16",
                        bgMain = "#0e1507",
                        bgSecond = "#16220b"
                    }),
                    SortOrder = 22,
                    CreatedAt = now,
                },
                new()
                {
                    Id = new Guid("40000000-0000-0000-0000-000000000024"),
                    Key = "chat-sakura",
                    Label = "Sakura Cherry",
                    Category = "General",
                    IsDefault = false,
                    IsActive = true,
                    IsEvent = false,
                    LightColorsJson = JsonSerializer.Serialize(new
                    {
                        gradient = "bg-gradient-to-tr from-[#db2777] to-[#fdf2f8]",
                        primaryLight = "#f472b6",
                        primaryMain = "#db2777",
                        bgMain = "#fff5f7",
                        bgSecond = "#ffe4e6"
                    }),
                    DarkColorsJson = JsonSerializer.Serialize(new
                    {
                        gradient = "bg-gradient-to-tr from-[#9d174d] to-[#f472b6]",
                        primaryLight = "#f472b6",
                        primaryMain = "#db2777",
                        bgMain = "#1c0d14",
                        bgSecond = "#2a131e"
                    }),
                    SortOrder = 23,
                    CreatedAt = now,
                },
            };

            modelBuilder.Entity<ChatTheme>().HasData(themes);
        }
    }
}

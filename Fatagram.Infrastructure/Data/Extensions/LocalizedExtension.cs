using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Fatagram.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Fatagram.Infrastructure.Data.Extensions
{
    public static class LocalizedExtension
    {
        public static void AddLocalized(this ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Localized>()
                .HasData(
                    // === HOBBY

                    // en

                    new Localized
                    {
                        Id = new Guid("11111111-1111-1111-1111-111111111111"),
                        LanguageCode = "en",
                        LocalizationKey = "HOBBY_MUSIC",
                        Value = "Music",
                    },
                    new Localized
                    {
                        Id = new Guid("22222222-2222-2222-2222-222222222222"),
                        LanguageCode = "en",
                        LocalizationKey = "HOBBY_SPORTS",
                        Value = "Sports",
                    },
                    new Localized
                    {
                        Id = new Guid("33333333-3333-3333-3333-333333333333"),
                        LanguageCode = "en",
                        LocalizationKey = "HOBBY_TRAVEL",
                        Value = "Travel",
                    },
                    new Localized
                    {
                        Id = new Guid("44444444-4444-4444-4444-444444444444"),
                        LanguageCode = "en",
                        LocalizationKey = "HOBBY_READING",
                        Value = "Reading",
                    },
                    // vi

                    new Localized
                    {
                        Id = new Guid("55555555-5555-5555-5555-555555555555"),
                        LanguageCode = "vi",
                        LocalizationKey = "HOBBY_MUSIC",
                        Value = "Âm nhạc",
                    },
                    new Localized
                    {
                        Id = new Guid("66666666-6666-6666-6666-666666666666"),
                        LanguageCode = "vi",
                        LocalizationKey = "HOBBY_SPORTS",
                        Value = "Thể thao",
                    },
                    new Localized
                    {
                        Id = new Guid("77777777-7777-7777-7777-777777777777"),
                        LanguageCode = "vi",
                        LocalizationKey = "HOBBY_TRAVEL",
                        Value = "Du lịch",
                    },
                    new Localized
                    {
                        Id = new Guid("88888888-8888-8888-8888-888888888888"),
                        LanguageCode = "vi",
                        LocalizationKey = "HOBBY_READING",
                        Value = "Đọc sách",
                    },
                    // === SKILL
                    // en

                    new Localized
                    {
                        Id = new Guid("99999999-9999-9999-9999-999999999999"),
                        LanguageCode = "en",
                        LocalizationKey = "SKILL_PROGRAMMING",
                        Value = "Programming",
                    },
                    new Localized
                    {
                        Id = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                        LanguageCode = "en",
                        LocalizationKey = "SKILL_DESIGN",
                        Value = "Design",
                    },
                    new Localized
                    {
                        Id = new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                        LanguageCode = "en",
                        LocalizationKey = "SKILL_MANAGEMENT",
                        Value = "Management",
                    },
                    new Localized
                    {
                        Id = new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                        LanguageCode = "en",
                        LocalizationKey = "SKILL_MARKETING",
                        Value = "Marketing",
                    },
                    // vi

                    new Localized
                    {
                        Id = new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"),
                        LanguageCode = "vi",
                        LocalizationKey = "SKILL_PROGRAMMING",
                        Value = "Lập trình",
                    },
                    new Localized
                    {
                        Id = new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"),
                        LanguageCode = "vi",
                        LocalizationKey = "SKILL_DESIGN",
                        Value = "Thiết kế",
                    },
                    new Localized
                    {
                        Id = new Guid("00000000-0000-0000-0000-000000000001"),
                        LanguageCode = "vi",
                        LocalizationKey = "SKILL_MANAGEMENT",
                        Value = "Quản lý",
                    },
                    new Localized
                    {
                        Id = new Guid("00000000-0000-0000-0000-000000000002"),
                        LanguageCode = "vi",
                        LocalizationKey = "SKILL_MARKETING",
                        Value = "Tiếp thị",
                    }
                );
        }
    }
}

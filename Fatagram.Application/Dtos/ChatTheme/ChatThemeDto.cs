using System;

namespace Fatagram.Application.Dtos.ChatTheme
{
    public class ChatThemeDto
    {
        public Guid Id { get; set; }
        public string Key { get; set; } = null!;
        public string Label { get; set; } = null!;
        public string Category { get; set; } = "General";
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; }
        public bool IsEvent { get; set; }
        public string? BgImage { get; set; }
        public ThemeColorsDto? Light { get; set; }
        public ThemeColorsDto? Dark { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int SortOrder { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}

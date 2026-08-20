using System;

namespace Fatagram.Domain.Models
{
    public class ChatTheme : BaseEntity
    {
        public string Key { get; set; } = null!;
        public string Label { get; set; } = null!;
        public string Category { get; set; } = "General";
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsEvent { get; set; }
        public string? BgImage { get; set; }
        public string? LightColorsJson { get; set; }
        public string? DarkColorsJson { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int SortOrder { get; set; }
    }
}

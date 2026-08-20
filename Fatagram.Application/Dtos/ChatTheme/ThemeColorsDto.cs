using System.Text.Json.Serialization;

namespace Fatagram.Application.Dtos.ChatTheme
{
    public class ThemeColorsDto
    {
        [JsonPropertyName("gradient")]
        public string? Gradient { get; set; }

        [JsonPropertyName("primaryLight")]
        public string? PrimaryLight { get; set; }

        [JsonPropertyName("primaryMain")]
        public string? PrimaryMain { get; set; }

        [JsonPropertyName("bgMain")]
        public string? BgMain { get; set; }

        [JsonPropertyName("bgSecond")]
        public string? BgSecond { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Fatagram.Application.Utils;

namespace Fatagram.Application.Dtos.Auth
{
    /// <summary>
    /// Login DTO
    /// </summary>
    public class LoginDto
    {
        [JsonPropertyName("usernameOrEmail")]
        public string UsernameOrEmail { get; set; } = string.Empty;

        [JsonPropertyName("password")]
        public string Password { get; set; } = string.Empty;
    }
}

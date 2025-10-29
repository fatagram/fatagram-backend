using System.ComponentModel.DataAnnotations;
using Fatagram.Application.Utils;

namespace Fatagram.Application.Dtos.Auth
{
    /// <summary>
    /// Login DTO
    /// </summary>
    public class LoginDto
    {
        public string Username { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
    }
}

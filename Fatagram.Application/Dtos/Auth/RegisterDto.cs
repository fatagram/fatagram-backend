using System.ComponentModel.DataAnnotations;
using Fatagram.Application.Utils;
using Fatagram.Domain.Enums;

namespace Fatagram.Application.Dtos.Auth
{
    /// <summary>
    /// Data Transfer Object for user registration.
    /// </summary>
    public class RegisterDto
    {
        public string Username { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;
    }
}

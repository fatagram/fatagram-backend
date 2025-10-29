using System.ComponentModel.DataAnnotations;
using Fatagram.Application.Utils;
using Fatagram.Domain.Enums;

namespace Fatagram.Application.Dtos.Account
{
    /// <summary>
    /// Data Transfer Object for user registration.
    /// </summary>
    public class RegisterDto
    {
        public string Username { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string? Phone { get; set; }

        public DateTime? BirthDay { get; set; }

        public Gender? Gender { get; set; } = Domain.Enums.Gender.Other;
    }
}

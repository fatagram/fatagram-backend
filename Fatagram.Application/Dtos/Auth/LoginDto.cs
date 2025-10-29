using System.ComponentModel.DataAnnotations;
using Fatagram.Application.Utils;
using Fatagram.Application.Utils.Errors;

namespace Fatagram.Application.Dtos.Auth
{
    /// <summary>
    /// Login DTO
    /// </summary>
    public class LoginDto
    {
        [RegularExpression(
            @"^[0-9a-zA-Z]{2,}$",
            ErrorMessage = AuthErrors.USERNAME_NOT_CORRECT_FORMAT
        )]
        [Required(ErrorMessage = AuthErrors.EMPTY_USERNAME)]
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Password
        /// </summary>
        [RegularExpression(@"^\S{8,}$", ErrorMessage = AuthErrors.INCORRECT_PASSWORD)]
        [Required(ErrorMessage = AuthErrors.EMPTY_PASSWORD)]
        public string Password { get; set; } = string.Empty;
    }
}

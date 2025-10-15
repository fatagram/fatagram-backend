using Fatagram.Application.Utils;
using System.ComponentModel.DataAnnotations;

namespace Fatagram.Application.Dtos.Auth
{

    /// <summary>
    /// Login DTO
    /// </summary>
    public class LoginDto
    {
        /// <summary>
        /// Username
        /// </summary>
        [RegularExpression(@"^[0-9a-zA-Z]{2,}$", ErrorMessage = ErrorCodes.USERNAME_NOT_CORRECT_FORMAT)]
        [Required(ErrorMessage = ErrorCodes.USERNAME_NOT_CORRECT_FORMAT)]
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Password
        /// </summary>
        [RegularExpression(@"^\S{8,}$", ErrorMessage = ErrorCodes.PASSWORD_NOT_CORRECT_FORMAT)]
        [Required(ErrorMessage = ErrorCodes.PASSWORD_NOT_CORRECT_FORMAT)]
        public string Password { get; set; } = string.Empty;
    }
}

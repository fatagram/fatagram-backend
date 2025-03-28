using Fatagram.Application.Utils;
using Fatagram.Domain.Utils;
using System.ComponentModel.DataAnnotations;

namespace Fatagram.Application.Dtos.Auth
{
    /// <summary>
    /// Data transfer object for changing password
    /// </summary>
    public class ChangePasswordDto
    {
        /// <summary>
        /// Old password
        /// </summary>
        [RegularExpression(@"^\S{9,}$", ErrorMessage = ErrorCodes.PASSWORD_NOT_CORRECT_FORMAT)]
        [Required(ErrorMessage = ErrorCodes.PASSWORD_NOT_CORRECT_FORMAT)]
        public string OldPassword { get; set; } = string.Empty;

        /// <summary>
        /// New password
        /// </summary>
        [RegularExpression(@"^\S{9,}$", ErrorMessage = ErrorCodes.PASSWORD_NOT_CORRECT_FORMAT)]
        [Required(ErrorMessage = ErrorCodes.PASSWORD_NOT_CORRECT_FORMAT)]
        public string NewPassword { get; set; } = string.Empty;
    }
}

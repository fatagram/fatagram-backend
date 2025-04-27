using Fatagram.Application.Utils;
using Fatagram.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Fatagram.Application.Dtos.Account
{
    /// <summary>
    /// Data Transfer Object for user registration.
    /// </summary>
    public class RegisterDto
    {
        /// <summary>
        /// Gets or sets the username for the account.
        /// </summary>
        [RegularExpression(RegexPatterrns.Username, ErrorMessage = ErrorCodes.USERNAME_NOT_CORRECT_FORMAT)]
        [Required(ErrorMessage = ErrorCodes.USERNAME_NOT_CORRECT_FORMAT)]
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the password for the account.
        /// </summary>
        [RegularExpression(RegexPatterrns.Password, ErrorMessage = ErrorCodes.PASSWORD_NOT_CORRECT_FORMAT)]
        [Required(ErrorMessage = ErrorCodes.PASSWORD_NOT_CORRECT_FORMAT)]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the email address for the account.
        /// </summary>
        [RegularExpression(RegexPatterrns.Email, ErrorMessage = ErrorCodes.EMAIL_NOT_CORRECT_FORMAT)]
        [Required(ErrorMessage = ErrorCodes.EMAIL_NOT_CORRECT_FORMAT)]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the first name of the user.
        /// </summary>
        [RegularExpression(RegexPatterrns.FirstName, ErrorMessage = ErrorCodes.FIRSTNAME_NOT_CORRECT_FORMAT)]
        [Required(ErrorMessage = ErrorCodes.FIRSTNAME_NOT_CORRECT_FORMAT)]
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the last name of the user.
        /// </summary>
        [RegularExpression(RegexPatterrns.LastName, ErrorMessage = ErrorCodes.LASTNAME_NOT_CORRECT_FORMAT)]
        [Required(ErrorMessage = ErrorCodes.LASTNAME_NOT_CORRECT_FORMAT)]
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the phone number of the user.
        /// </summary>
        [RegularExpression(RegexPatterrns.Phone, ErrorMessage = ErrorCodes.PHONE_NUMBER_NOT_CORRECT_FORMAT)]
        public string? Phone { get; set; }

        public DateTime? BirthDay { get; set; }

        public Gender? Gender { get; set; } = Domain.Enums.Gender.Other;
    }
}

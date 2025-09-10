using Fatagram.Application.Utils;
using SixLabors.ImageSharp.PixelFormats;
using System.ComponentModel.DataAnnotations;

namespace Fatagram.Application.Dtos.User.Update
{
    /// <summary>
    /// Data transfer object for updating user
    /// </summary>
    public class UpdateUserDto
    {
        [RegularExpression(RegexPatterns.Username)]
        [Required]
        public string? Username { get; set; }

        /// <summary>
        /// User's first name
        /// </summary>
        [RegularExpression(RegexPatterns.FirstName, ErrorMessage = ErrorCodes.FIRSTNAME_NOT_CORRECT_FORMAT)]
        [Required(ErrorMessage = ErrorCodes.FIRSTNAME_NOT_CORRECT_FORMAT)]
        public string? FirstName { get; set; }

        /// <summary>
        /// User's last name
        /// </summary>
        /// 
        [RegularExpression(RegexPatterns.LastName, ErrorMessage = ErrorCodes.LASTNAME_NOT_CORRECT_FORMAT)]
        [Required(ErrorMessage = ErrorCodes.LASTNAME_NOT_CORRECT_FORMAT)]
        public string? LastName { get; set; }

        /// <summary>
        /// User's email
        /// </summary>
        /// 
        [RegularExpression(RegexPatterns.Email, ErrorMessage = ErrorCodes.EMAIL_NOT_CORRECT_FORMAT)]
        [Required(ErrorMessage = ErrorCodes.EMAIL_NOT_CORRECT_FORMAT)]
        public string? Email { get; set; }

        /// <summary>
        /// User's bio
        /// </summary>
        [RegularExpression(RegexPatterns.Bio, ErrorMessage = ErrorCodes.BIO_TOO_LONG)]
        // [Required(ErrorMessage = ErrorCodes.BIO_TOO_LONG)]
        public string? Bio { get; set; }

        /// <summary>
        /// User's description
        /// </summary>
        [RegularExpression(RegexPatterns.Description, ErrorMessage = ErrorCodes.DESCRIPTION_TOO_LONG)]
        public string? Description { get; set; }

        /// <summary>
        /// User's avatar
        /// </summary>
        public string? Avatar { get; set; }

        public string? Background { get; set; }

        /// <summary>
        /// User's phone number
        /// </summary>
        /// 
        [RegularExpression(RegexPatterns.Phone, ErrorMessage = ErrorCodes.PHONE_NUMBER_NOT_CORRECT_FORMAT)]
        public string? Phone { get; set; }
    }
}

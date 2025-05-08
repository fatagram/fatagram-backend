using Fatagram.Application.Utils;
using SixLabors.ImageSharp.PixelFormats;
using System.ComponentModel.DataAnnotations;

namespace Fatagram.Application.Dtos.User
{
    /// <summary>
    /// Data transfer object for updating user
    /// </summary>
    public class UpdateUserDto
    {
        [RegularExpression(RegexPatterrns.Username)]
        [Required]
        public string? Username { get; set; }

        /// <summary>
        /// User's first name
        /// </summary>
        [RegularExpression(RegexPatterrns.FirstName, ErrorMessage = ErrorCodes.FIRSTNAME_NOT_CORRECT_FORMAT)]
        [Required(ErrorMessage = ErrorCodes.FIRSTNAME_NOT_CORRECT_FORMAT)]
        public string? FirstName { get; set; }

        /// <summary>
        /// User's last name
        /// </summary>
        /// 
        [RegularExpression(RegexPatterrns.LastName, ErrorMessage = ErrorCodes.LASTNAME_NOT_CORRECT_FORMAT)]
        [Required(ErrorMessage = ErrorCodes.LASTNAME_NOT_CORRECT_FORMAT)]
        public string? LastName { get; set; }

        /// <summary>
        /// User's email
        /// </summary>
        /// 
        [RegularExpression(RegexPatterrns.Email, ErrorMessage = ErrorCodes.EMAIL_NOT_CORRECT_FORMAT)]
        [Required(ErrorMessage = ErrorCodes.EMAIL_NOT_CORRECT_FORMAT)]
        public string? Email { get; set; }

        /// <summary>
        /// User's bio
        /// </summary>
        public string? Bio { get; set; }

        /// <summary>
        /// User's avatar
        /// </summary>
        public string? Avatar { get; set; }

        public string? Background { get; set; }

        /// <summary>
        /// User's phone number
        /// </summary>
        /// 
        [RegularExpression(RegexPatterrns.Phone, ErrorMessage = ErrorCodes.PHONE_NUMBER_NOT_CORRECT_FORMAT)]
        public string? Phone { get; set; }
    }
}

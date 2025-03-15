using Fatagram.Application.Utils;
using System.ComponentModel.DataAnnotations;

namespace Fatagram.Application.Dtos
{
    /// <summary>
    /// Data transfer object for updating user
    /// </summary>
    public class UpdateUserDto
    {
        /// <summary>
        /// User's first name
        /// </summary>
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = ErrorCodes.FIRSTNAME_NOT_CORRECT_FORMAT)]
        [Required(ErrorMessage = ErrorCodes.FIRSTNAME_NOT_CORRECT_FORMAT)]
        public string? FirstName { get; set; }

        /// <summary>
        /// User's last name
        /// </summary>
        /// 
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = ErrorCodes.LASTNAME_NOT_CORRECT_FORMAT)]
        [Required(ErrorMessage = ErrorCodes.LASTNAME_NOT_CORRECT_FORMAT)]
        public string? LastName { get; set; }

        /// <summary>
        /// User's email
        /// </summary>
        /// 
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = ErrorCodes.EMAIL_NOT_CORRECT_FORMAT)]
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

        /// <summary>
        /// User's phone number
        /// </summary>
        /// 
        [RegularExpression(@"^[0-9]{10,}$", ErrorMessage = ErrorCodes.PHONE_NUMBER_NOT_CORRECT_FORMAT)]
        public string? Phone { get; set; }
    }
}

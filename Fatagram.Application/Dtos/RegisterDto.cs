using Fatagram.Application.Utils;
using System.ComponentModel.DataAnnotations;

namespace Fatagram.Application.Dtos
{
    public class RegisterDto
    {

        [RegularExpression(@"^[0-9a-zA-Z]{2,}$", ErrorMessage = ErrorCodes.USERNAME_NOT_CORRECT_FORMAT)]
        [Required(ErrorMessage = ErrorCodes.USERNAME_NOT_CORRECT_FORMAT)]
        public string Username { get; set; } = string.Empty;



        [RegularExpression(@"^\S{9,}$", ErrorMessage = ErrorCodes.PASSWORD_NOT_CORRECT_FORMAT)]
        [Required(ErrorMessage = ErrorCodes.PASSWORD_NOT_CORRECT_FORMAT)]
        public string Password { get; set; } = string.Empty;



        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = ErrorCodes.EMAIL_NOT_CORRECT_FORMAT)]
        [Required(ErrorMessage = ErrorCodes.EMAIL_NOT_CORRECT_FORMAT)]
        public string Email { get; set; } = string.Empty;



        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = ErrorCodes.FIRSTNAME_NOT_CORRECT_FORMAT)]
        [Required(ErrorMessage = ErrorCodes.FIRSTNAME_NOT_CORRECT_FORMAT)]
        public string FirstName { get; set; } = string.Empty;



        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = ErrorCodes.LASTNAME_NOT_CORRECT_FORMAT)]
        [Required(ErrorMessage = ErrorCodes.LASTNAME_NOT_CORRECT_FORMAT)]
        public string LastName { get; set; } = string.Empty;



        [RegularExpression(@"^[0-9]{10,}$", ErrorMessage = ErrorCodes.PHONE_NUMBER_NOT_CORRECT_FORMAT)]
        public string? Phone { get; set; }
    }
}

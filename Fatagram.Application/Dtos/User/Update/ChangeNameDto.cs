using Fatagram.Application.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fatagram.Application.Dtos.User.Update
{
    public class ChangeNameDto
    {
        [RegularExpression(RegexPatterns.FirstName, ErrorMessage = ErrorCodes.FIRSTNAME_NOT_CORRECT_FORMAT)]
        [Required(ErrorMessage = ErrorCodes.FIRSTNAME_NOT_CORRECT_FORMAT)]
        public string FirstName { get; set; } = null!;

        [RegularExpression(RegexPatterns.LastName, ErrorMessage = ErrorCodes.LASTNAME_NOT_CORRECT_FORMAT)]
        [Required(ErrorMessage = ErrorCodes.LASTNAME_NOT_CORRECT_FORMAT)]
        public string LastName { get; set; } = null!;
    }
}

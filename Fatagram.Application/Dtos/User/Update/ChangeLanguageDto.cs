using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fatagram.Application.Dtos.User.Update
{
    public class ChangeLanguageDto
    {
        [Required]
        [RegularExpression("^(en|vi)$", ErrorMessage = "Language code must be 'en' or 'vi'.")]
        public string LanguageCode { get; set; } = "en";
    }
}

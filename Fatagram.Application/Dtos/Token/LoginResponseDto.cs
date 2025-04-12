using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fatagram.Application.Dtos.Token
{
    public class LoginResponseDto
    {
        [Required]
        public string RefreshToken { get; set; } = string.Empty;
        
        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public string? Username { get; set; }
    }
}

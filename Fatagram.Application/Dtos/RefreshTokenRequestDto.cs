using System.ComponentModel.DataAnnotations;

namespace Fatagram.Application.Dtos
{
    public class RefreshTokenRequestDto
    {
        [Required]
        public string RefreshToken { get; set; } = string.Empty;
    }
}

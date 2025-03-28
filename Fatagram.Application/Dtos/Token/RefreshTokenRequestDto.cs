using System.ComponentModel.DataAnnotations;

namespace Fatagram.Application.Dtos.Token
{
    public class RefreshTokenRequestDto
    {
        [Required]
        public string RefreshToken { get; set; } = string.Empty;
    }
}

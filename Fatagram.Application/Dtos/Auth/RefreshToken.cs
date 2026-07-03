using System.ComponentModel.DataAnnotations;

namespace Fatagram.Application.Dtos.Token
{
    public record RefreshTokenRequestDto
    {
        public string RefreshToken { get; set; } = string.Empty;
    }

    public record RefreshTokenInfo
    {
        public Guid UserId { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}

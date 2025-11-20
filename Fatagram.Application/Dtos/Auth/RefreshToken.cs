using System.ComponentModel.DataAnnotations;

namespace Fatagram.Application.Dtos.Token
{
    public class RefreshTokenRequestDto
    {
        public string RefreshToken { get; set; } = string.Empty;
    }

    public class RefreshTokenInfo
    {
        public Guid AccountId { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}

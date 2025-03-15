using Fatagram.Domain.Utils;

namespace Fatagram.Application.Services
{
    public class RefreshTokenService
    {
        /// <summary>
        /// Generate a new refresh token
        /// </summary>
        /// <returns></returns>
        public static RefreshTokenGenerateResult GenerateRefreshToken()
        {
            return new()
            {
                Token = Guid.NewGuid().ToString(),
                ExpiryDate = DateTime.UtcNow.AddDays(7),
                CreateDate = DateTime.UtcNow
            };
        }
    }
}

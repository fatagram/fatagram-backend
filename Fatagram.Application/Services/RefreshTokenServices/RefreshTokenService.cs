using Fatagram.Application.Utils;

namespace Fatagram.Application.Services.RefreshTokenServices
{
    public static class RefreshTokenService
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
                CreateDate = DateTime.UtcNow,
            };
        }
    }
}

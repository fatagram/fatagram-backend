using Fatagram.Application.Dtos.Token;

namespace Fatagram.Application.Services.TokenServices
{
    /// <summary>
    /// Interface for the token service
    /// </summary>
    public interface ITokenService
    {
        /// <summary>
        /// Generate a new access token and refresh token
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        string GenerateAccessToken(Guid userId);

        /// <summary>
        /// Generate a new refresh token
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<string> GenerateRefreshTokenAsync(Guid userId);

        /// <summary>
        /// Generate a new access token from a refresh token
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        Task<string?> GenerateAccessTokenFromRefreshTokenAsync(string refreshToken);

        /// <summary>
        /// Delete a refresh token
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        Task DeleteRefreshTokenAsync(string refreshToken);
    }
}

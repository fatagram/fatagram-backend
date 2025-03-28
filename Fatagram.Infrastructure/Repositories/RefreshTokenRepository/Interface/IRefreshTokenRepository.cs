using Fatagram.Domain.Models;
using Fatagram.Shared.Utils;

namespace Fatagram.Infrastructure.Repositories.RefreshTokenRepository.Interface
{
    /// <summary>
    /// Interface for the refresh token repository
    /// </summary>
    public interface IRefreshTokenRepository
    {
        /// <summary>
        /// Create a new refresh token
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="refreshToken"></param>
        /// <param name="expiredTime"></param>
        /// <param name="createdTime"></param>
        /// <returns></returns>
        Task CreateNewRefreshTokenAsync(RefreshToken refreshToken);

        /// <summary>
        /// Get the expiry time of a refresh token
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        Task<DateTime?> GetExpiryTimeAsync(string refreshToken);

        /// <summary>
        /// Get the account id of a refresh token
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        Task<Guid?> GetAccountIdAsync(string refreshToken);

        /// <summary>
        /// Delete a refresh token
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        Task DeleteRefreshTokenAsync(string refreshToken);
    }
}

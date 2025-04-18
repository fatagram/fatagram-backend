using Fatagram.Domain.Models;
using Fatagram.Infrastructure.Data;
using Fatagram.Infrastructure.Repositories.RefreshTokenRepository.Interface;
using Fatagram.Shared.Extensions;
using Fatagram.Shared.Utils;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Fatagram.Infrastructure.Repositories.RefreshTokenRepository
{
    /// <summary>
    /// Repository for managing refresh tokens.
    /// </summary>
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly AppDbContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="RefreshTokenRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        public RefreshTokenRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Creates a new refresh token asynchronously.
        /// </summary>
        /// <param name="accountId">The unique identifier of the account.</param>
        /// <param name="refreshToken">The refresh token.</param>
        /// <param name="expiredTime">The expiration time of the refresh token.</param>
        /// <param name="createdTime">The creation time of the refresh token.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the result of the operation.</returns>
        public async Task CreateNewRefreshTokenAsync(RefreshToken refreshToken)
        {
            await _dbContext.RefreshTokens.AddAsync(refreshToken);
            await _dbContext.SaveChangesAsync();
        }


        /// <summary>
        /// Deletes a refresh token asynchronously.
        /// </summary>
        /// <param name="refreshToken">The refresh token to delete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the result of the operation.</returns>
        public async Task DeleteRefreshTokenAsync(string refreshToken)
        {
            var token = await _dbContext.RefreshTokens.FindAsync(refreshToken.ToGuid());
            if (token == null)
            {
                throw new KeyNotFoundException();
            }
            _dbContext.RefreshTokens.Remove(token);
            await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Gets the account identifier associated with the specified refresh token asynchronously.
        /// </summary>
        /// <param name="refreshToken">The refresh token.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the account identifier if found, otherwise an error.</returns>
        public async Task<Guid?> GetAccountIdAsync(string refreshToken)
        {
            var tokenGuid = refreshToken.ToGuid();
            var token = await _dbContext.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == tokenGuid);
            return token?.AccountId;
        }

        /// <summary>
        /// Gets the expiration time of the specified refresh token asynchronously.
        /// </summary>
        /// <param name="refreshToken">The refresh token.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the expiration time if found, otherwise an error.</returns>
        public async Task<DateTime?> GetExpiryTimeAsync(string refreshToken)
        {
            var tokenGuid = refreshToken.ToGuid();
            var token = await _dbContext.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == tokenGuid);
            return token?.ExpiryDate;
        }
    }
}

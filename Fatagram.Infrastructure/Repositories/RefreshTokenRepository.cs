using Fatagram.Infrastructure.Data;
using Fatagram.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Fatagram.Infrastructure.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly AppDbContext _dbContext;

        public RefreshTokenRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Create a new refresh token
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="refreshToken"></param>
        /// <param name="expiredTime"></param>
        /// <param name="createdTime"></param>
        /// <returns></returns>
        public async Task<bool> CreateNewRefreshTokenAsync(Guid accountId, string refreshToken, DateTime expiredTime, DateTime createdAt)
        {
            //if (_dbContext.RefreshTokens.Any(x => x.AccountId == accountId))
            //{
            //    var refreshTokenEntity = _dbContext.RefreshTokens.First(x => x.AccountId == accountId);
                
            //    if (refreshTokenEntity != null)
            //    {
            //        refreshTokenEntity.Token = refreshToken;
            //        refreshTokenEntity.ExpiryDate = expiredTime;
            //        refreshTokenEntity.CreatedDate = createdTime;
            //        _dbContext.RefreshTokens.Update(refreshTokenEntity);
            //        await _dbContext.SaveChangesAsync();
            //        return true;
            //    }
            //    else
            //    {
            //        return false;
            //    }
            //}
            
            _dbContext.RefreshTokens.Add(new()
            {
                Token = refreshToken,
                AccountId = accountId,
                ExpiryDate = expiredTime,
                CreatedAt = createdAt
            });

            await _dbContext.SaveChangesAsync();
            return true;
        }


        /// <summary>
        /// Get the expiry time of a refresh token
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        public async Task<DateTime> GetExpiryTimeAsync(string refreshToken)
        {
            var refreshTokenEntity = await _dbContext.RefreshTokens.FirstOrDefaultAsync(x => x.Token == refreshToken);
            if (refreshTokenEntity == null) return DateTime.MinValue;

            return refreshTokenEntity.ExpiryDate;
        }


        /// <summary>
        /// Get the account id of a refresh token
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        public async Task<Guid> GetAccountIdAsync(string refreshToken)
        {
            var refreshTokenEntity = await _dbContext.RefreshTokens.FirstOrDefaultAsync(x => x.Token == refreshToken);
            if (refreshTokenEntity == null) return Guid.Empty;

            return refreshTokenEntity.AccountId;
        }


        /// <summary>
        /// Delete a refresh token
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        public async Task<bool> DeleteRefreshTokenAsync(string refreshToken)
        {
            var refreshTokenEntity = await _dbContext.RefreshTokens.FirstOrDefaultAsync(x => x.Token == refreshToken);
            if (refreshTokenEntity == null) return false;

            _dbContext.RefreshTokens.Remove(refreshTokenEntity);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public Task<bool> CreateNewRefreshTokenAsync(string accountId, string refreshToken, DateTime expiredTime, DateTime createdTime)
            => CreateNewRefreshTokenAsync(Guid.Parse(accountId), refreshToken, expiredTime, createdTime);
    }
}

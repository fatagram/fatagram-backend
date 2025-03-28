//using Fatagram.Domain.Models;
//using Fatagram.Infrastructure.Repositories.RefreshTokenRepository.Interface;
//using System.Diagnostics;

//namespace Fatagram.Infrastructure.Repositories.RefreshTokenRepository
//{
//    public class MRefreshTokenRepository : IRefreshTokenRepository
//    {
//        private List<RefreshToken> RefreshTokens { get; set; }

//        public MRefreshTokenRepository()
//        {
//            RefreshTokens = new List<RefreshToken>();
//        }

//        public Task<bool> CreateNewRefreshTokenAsync(Guid accountId, string refreshToken, DateTime expiredTime, DateTime createdAt)
//        {
//            RefreshTokens.Add(new RefreshToken()
//            {
//                Token = refreshToken,
//                AccountId = accountId,
//                ExpiryDate = expiredTime,
//                CreatedAt = createdAt
//            });

//            Debug.WriteLine("Refresh token created: " + refreshToken + "\nRefreshToken Count: " + RefreshTokens.Count);

//            return Task.FromResult(true);
//        }

//        public Task<bool> DeleteRefreshTokenAsync(string refreshToken)
//        {
//            var token = RefreshTokens.Find(t => t.Token == refreshToken);
//            if (token == null)
//            {
//                return Task.FromResult(false);
//            }

//            RefreshTokens.Remove(token);
//            return Task.FromResult(true);
//        }

//        public Task<Guid> GetAccountIdAsync(string refreshToken)
//        {
//            Debug.WriteLine("\nRefreshToken Count: " + RefreshTokens.Count);

//            var token = RefreshTokens.Find(t => t.Token == refreshToken);
//            if (token == null)
//            {
//                return Task.FromResult(Guid.Empty);
//            }

//            return Task.FromResult(token.AccountId);
//        }

//        public Task<DateTime> GetExpiryTimeAsync(string refreshToken)
//        {
//            var token = RefreshTokens.Find(t => t.Token == refreshToken);
//            if (token == null)
//            {
//                return Task.FromResult(DateTime.MinValue);
//            }

//            return Task.FromResult(token.ExpiryDate);
//        }

//        public Task<bool> CreateNewRefreshTokenAsync(string accountId, string refreshToken, DateTime expiredTime, DateTime createdTime)
//            => CreateNewRefreshTokenAsync(Guid.Parse(accountId), refreshToken, expiredTime, createdTime);
//    }
//}

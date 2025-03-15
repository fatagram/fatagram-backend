using Fatagram.Application.Dtos;
using Fatagram.Application.Services.Interfaces;
using Fatagram.Application.Utils;
using Fatagram.Infrastructure.Repositories.Interfaces;
using System.Runtime.CompilerServices;

namespace Fatagram.Application.Services
{
    public class TokenService : ITokenService
    {
        private readonly IJwtService _jwtService;
        private readonly IAccountRepository _accountRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public TokenService(IJwtService jwtService, IAccountRepository accountRepository, IRefreshTokenRepository refreshTokenRepository)
        {
            _jwtService = jwtService;
            _accountRepository = accountRepository;
            _refreshTokenRepository = refreshTokenRepository;
        }



        /// <summary>
        /// Generate tokens for a user
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public async Task<Result<TokenDto>> GenerateTokensAsync(string username)
        {
            var account = await _accountRepository.GetAccountByUsernameAsync(username);
            if (account is null) return Result<TokenDto>.Failure("GENERATE_TOKEN_FAILED");

            var refreshToken = RefreshTokenService.GenerateRefreshToken().Token;
            await _refreshTokenRepository.CreateNewRefreshTokenAsync(account.Id, refreshToken, DateTime.UtcNow.AddDays(7), DateTime.UtcNow);

            var accessToken = _jwtService.GenerateToken(username, account.UserId).Data;
            if (accessToken is null) return Result<TokenDto>.Failure("GENERATE_TOKEN_FAILED");

            return Result<TokenDto>.Success(new()
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            });
        }


        /// <summary>
        /// Validate a refresh token
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<Result<string>> ValidateRefreshToken(string refreshToken)
        {
            var accountId = await _refreshTokenRepository.GetAccountIdAsync(refreshToken);
            if (accountId == Guid.Empty) return Result<string>.Failure(ErrorCodes.REFRESH_TOKEN_INVALID);

            var expiryTime = await _refreshTokenRepository.GetExpiryTimeAsync(refreshToken);
            if (expiryTime < DateTime.UtcNow) return Result<string>.Failure(ErrorCodes.REFRESH_TOKEN_INVALID);

            return Result<string>.Success("REFRESH_TOKEN_VALID");
        }




        /// <summary>
        /// Refresh the access token
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        public async Task<Result<string>> RefreshAccessTokenAsync(string refreshToken)
        {
            // Find accound id by refresh token
            var accountId = await _refreshTokenRepository.GetAccountIdAsync(refreshToken);
            if (accountId == Guid.Empty) return Result<string>.Failure(ErrorCodes.ACCOUNT_NOT_FOUND);

            // Find account by account id
            var account = await _accountRepository.GetAccountByIdAsync(accountId);
            if (account == null) return Result<string>.Failure(ErrorCodes.ACCOUNT_NOT_FOUND);

            // Check if refresh token is expired
            var expiryTime = await _refreshTokenRepository.GetExpiryTimeAsync(refreshToken);
            if (expiryTime < DateTime.UtcNow) return Result<string>.Failure(ErrorCodes.REFRESH_TOKEN_EXPIRED);

            return _jwtService.GenerateToken(account.Username, account.UserId);
        }


        /// <summary>
        /// Delete a refresh token
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        public async Task<Result<string>> DeleteRefreshTokenAsync(string refreshToken)
        {
            var res = await _refreshTokenRepository.DeleteRefreshTokenAsync(refreshToken);
            if (res) return Result<string>.Success("REFRESH_TOKEN_DELETED");
            return Result<string>.Failure(ErrorCodes.REFRESH_TOKEN_DELETE_FAILED);
        }
    }
}

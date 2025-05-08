using Fatagram.Application.Dtos.Token;
using Fatagram.Application.Exceptions;
using Fatagram.Application.Services.JwtServices.Interface;
using Fatagram.Application.Services.RefreshTokenServices;
using Fatagram.Application.Services.TokenServices.Interface;
using Fatagram.Application.Utils;
using Fatagram.Domain.Models;
using Fatagram.Infrastructure.Repositories.AccountRepository.Interface;
using Fatagram.Infrastructure.Repositories.RefreshTokenRepository.Interface;
using Fatagram.Shared.Extensions;
using Fatagram.Shared.Utils;
using System.Runtime.CompilerServices;

namespace Fatagram.Application.Services.TokenServices
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
            var res = await _accountRepository.GetAccountByUsernameAsync(username);
            if (res is null) throw new AccountNotFoundException();
            var account = res;

            var refreshToken = RefreshTokenService.GenerateRefreshToken().Token;
            await _refreshTokenRepository.CreateNewRefreshTokenAsync(new RefreshToken()
            {
                AccountId = account.Id,
                Token = refreshToken.ToGuid(),
                ExpiryDate = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow
            });

            var accessToken = _jwtService.GenerateToken(username, account.UserId).Data;
            if (accessToken is null) 
                throw new AppException("GENERATE_ACCESS_TOKEN_FAILED");

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
            var res = await _refreshTokenRepository.GetAccountIdAsync(refreshToken);
            if (res is null) throw new AppException(ErrorCodes.REFRESH_TOKEN_INVALID);

            var rtExpiredTime = await _refreshTokenRepository.GetExpiryTimeAsync(refreshToken);

            if (rtExpiredTime < DateTime.UtcNow) throw new AppException(ErrorCodes.REFRESH_TOKEN_INVALID);

            return Result<string>.Success();
        }

        /// <summary>
        /// Refresh the access token
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        public async Task<Result<string>> RefreshAccessTokenAsync(string refreshToken)
        {
            // Find accound id by refresh token
            var res = await _refreshTokenRepository.GetAccountIdAsync(refreshToken);
            if (res is null) throw new AppException("REFRESH_TOKEN_NOT_EXIST");

            // Find account by account id
            var getAccountResult = await _accountRepository.GetAccountByIdAsync((Guid)res);
            if (getAccountResult is null) throw new AccountNotFoundException();
            var account = getAccountResult;

            // Check if refresh token is expired
            var rtExpiredTime = await _refreshTokenRepository.GetExpiryTimeAsync(refreshToken);
            if (rtExpiredTime < DateTime.UtcNow) throw new AppException(ErrorCodes.REFRESH_TOKEN_EXPIRED);

            var token = _jwtService.GenerateToken(account.Username, account.UserId);
            return Result<string>.Success(token.Data);
        }

        /// <summary>
        /// Delete a refresh token
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        public async Task<Result<string>> DeleteRefreshTokenAsync(string refreshToken)
        {
            await _refreshTokenRepository.DeleteRefreshTokenAsync(refreshToken);
            return Result<string>.Success();
        }
    }
}

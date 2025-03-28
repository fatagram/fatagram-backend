using Fatagram.Application.Dtos.Token;
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
            try
            {
                var res = await _accountRepository.GetAccountByUsernameAsync(username);
                if (res is null) return Result<TokenDto>.Failure("GENERATE_TOKEN_FAILED");
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
                if (accessToken is null) return Result<TokenDto>.Failure("GENERATE_TOKEN_FAILED");

                return Result<TokenDto>.Success(new()
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken
                });
            }
            catch (Exception)
            {
                return Result<TokenDto>.Failure("GENERATE_TOKEN_FAILED");
            }
        }

        /// <summary>
        /// Validate a refresh token
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<Result<string>> ValidateRefreshToken(string refreshToken)
        {
            try
            {
                var res = await _refreshTokenRepository.GetAccountIdAsync(refreshToken);
                if (res is null) return Result<string>.Failure(ErrorCodes.REFRESH_TOKEN_INVALID);

                var rtExpiredTime = await _refreshTokenRepository.GetExpiryTimeAsync(refreshToken);

                if (rtExpiredTime < DateTime.UtcNow) return Result<string>.Failure(ErrorCodes.REFRESH_TOKEN_INVALID);

                return Result<string>.Success("REFRESH_TOKEN_VALID");
            }
            catch (Exception)
            {
                return Result<string>.Failure("REFRESH_TOKEN_INVALID");
            }
            
        }

        /// <summary>
        /// Refresh the access token
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        public async Task<Result<string>> RefreshAccessTokenAsync(string refreshToken)
        {
            try
            {
                // Find accound id by refresh token
                var res = await _refreshTokenRepository.GetAccountIdAsync(refreshToken);
                if (res is null) return Result<string>.Failure(ErrorCodes.ACCOUNT_NOT_FOUND);

                // Find account by account id
                var getAccountResult = await _accountRepository.GetAccountByIdAsync((Guid)res);
                if (getAccountResult is null) return Result<string>.Failure(ErrorCodes.ACCOUNT_NOT_FOUND);
                var account = getAccountResult;

                // Check if refresh token is expired
                var rtExpiredTime = await _refreshTokenRepository.GetExpiryTimeAsync(refreshToken);
                if (rtExpiredTime < DateTime.UtcNow) return Result<string>.Failure(ErrorCodes.REFRESH_TOKEN_EXPIRED);

                var token = _jwtService.GenerateToken(account.Username, account.UserId);
                return Result<string>.Success(token.Data);
            }
            catch (Exception)
            {
                return Result<string>.Failure("REFRESH_ACCESS_TOKEN_FAILED");
            }
            
        }

        /// <summary>
        /// Delete a refresh token
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        public async Task<Result<string>> DeleteRefreshTokenAsync(string refreshToken)
        {
            try
            {
                await _refreshTokenRepository.DeleteRefreshTokenAsync(refreshToken);
                return Result<string>.Success("DELETE_REFRESH_TOKEN_SUCCESS");
            }
            catch (Exception)
            {
                return Result<string>.Failure("DELETE_REFRESH_TOKEN_FAILED");
            }
        }
    }
}

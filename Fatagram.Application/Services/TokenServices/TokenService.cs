using System.Drawing;
using System.Linq.Dynamic.Core.Tokenizer;
using System.Runtime.CompilerServices;
using Fatagram.Application.Dtos.Token;
using Fatagram.Application.Exceptions;
using Fatagram.Application.Exceptions.DetailExceptions;
using Fatagram.Application.Services.JwtServices.Interface;
using Fatagram.Application.Services.RefreshTokenServices;
using Fatagram.Application.Services.TokenServices.Interface;
using Fatagram.Application.Utils;
using Fatagram.Domain.Models;
using Fatagram.Infrastructure.Repositories.AccountRepository.Interface;
using Fatagram.Infrastructure.Repositories.RefreshTokenRepository.Interface;
using Fatagram.Shared.Enums;
using Fatagram.Shared.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;

namespace Fatagram.Application.Services.TokenServices
{
    public class TokenService(
        IJwtService jwtService,
        IAccountRepository accountRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IConfiguration configuration,
        ILogger<TokenService> logger
    ) : ITokenService
    {
        private readonly IJwtService _jwtService = jwtService;
        private readonly IAccountRepository _accountRepository = accountRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository = refreshTokenRepository;
        private readonly IConfiguration _configuration = configuration;
        private readonly ILogger<TokenService> _logger = logger;

        /// <summary>
        /// Generate tokens for a user
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public async Task<string> GenerateAccessTokenAsync(Guid accountId)
        {
            var userAccount =
                await _accountRepository.GetAsync<Account>(accountId)
                ?? throw new AccountNotFoundException();
            var token =
                _jwtService.GenerateToken(userAccount.UserId) ?? throw new GenerateTokenException();
            return token.Data!;
        }

        public async Task<string> GenerateRefreshTokenAsync(Guid accountId)
        {
            var newRefreshToken = Guid.NewGuid();
            await _refreshTokenRepository.AddAsync(
                new RefreshToken
                {
                    AccountId = accountId,
                    Token = newRefreshToken.ToString(),
                    ExpiresAt = DateTime.UtcNow.AddDays(
                        int.Parse(_configuration["JwtSettings:RefreshTokenExpireInDays"] ?? "7")
                    ),
                }
            );
            return newRefreshToken.ToString();
        }

        public async Task<string?> GenerateAccessTokenFromRefreshTokenAsync(string refreshToken)
        {
            var (res, accountId) = await ValidateRefreshTokenAsync(refreshToken);
            _logger.LogInformation("Refresh token is valid: {IsValid}", res);
            if (!res || accountId is null)
            {
                return null;
            }
            return await GenerateAccessTokenAsync(accountId.Value);
        }

        /// <summary>
        /// Validate a refresh token
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns>(IsValid, AccountId)</returns>
        /// <exception cref="NotImplementedException"></exception>
        async Task<(bool IsValid, Guid? AccountId)> ValidateRefreshTokenAsync(string refreshToken)
        {
            var tokenInfo = (
                await _refreshTokenRepository.GetByUniqueKeyAsync(
                    rt => rt.Token,
                    refreshToken,
                    rt => new RefreshTokenInfo()
                    {
                        AccountId = rt.AccountId,
                        ExpiresAt = rt.ExpiresAt,
                    }
                )
            );
            if (tokenInfo is null)
            {
                return (false, null);
            }
            if (tokenInfo.ExpiresAt < DateTime.UtcNow)
            {
                return (false, null);
            }
            return (true, tokenInfo.AccountId);
        }

        /// <summary>
        /// Delete a refresh token
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        public async Task DeleteRefreshTokenAsync(string refreshToken)
        {
            var rt = await _refreshTokenRepository.GetByUniqueKeyAsync(
                rt => rt.Token,
                refreshToken,
                rt => rt.AccountId
            );
            await _refreshTokenRepository.DeleteAsync(rt);
        }
    }
}

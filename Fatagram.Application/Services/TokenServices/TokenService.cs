using Fatagram.Application.Abstractions.Repositories;
using Fatagram.Application.Dtos.Token;
using Fatagram.Application.Exceptions.DetailExceptions;
using Fatagram.Application.Services.JwtServices;
using Fatagram.Domain.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Fatagram.Application.Services.TokenServices
{
    public class TokenService(
        IJwtService jwtService,
        IRefreshTokenRepository refreshTokenRepository,
        IConfiguration configuration,
        ILogger<TokenService> logger
    ) : ITokenService
    {
        private readonly IJwtService _jwtService = jwtService;
        private readonly IRefreshTokenRepository _refreshTokenRepository = refreshTokenRepository;
        private readonly IConfiguration _configuration = configuration;
        private readonly ILogger<TokenService> _logger = logger;

        /// <summary>
        /// Generate tokens for a user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public string GenerateAccessToken(Guid userId)
        {
            var token = _jwtService.GenerateToken(userId) ?? throw new GenerateTokenException();
            return token.Data!;
        }

        public async Task<string> GenerateRefreshTokenAsync(Guid userId)
        {
            var newRefreshToken = Guid.NewGuid();
            await _refreshTokenRepository.AddAsync(
                new RefreshToken
                {
                    UserId = userId,
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
            var (isValid, userId) = await ValidateRefreshTokenAsync(refreshToken);
            _logger.LogInformation("Refresh token is valid: {IsValid}", isValid);
            if (!isValid || userId is null)
            {
                return null;
            }
            return GenerateAccessToken(userId.Value);
        }

        /// <summary>
        /// Validate a refresh token
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns>(IsValid, UserId)</returns>
        /// <exception cref="NotImplementedException"></exception>
        async Task<(bool IsValid, Guid? UserId)> ValidateRefreshTokenAsync(string refreshToken)
        {
            var tokenInfos = (
                await _refreshTokenRepository.GetAllAsync<RefreshTokenInfo, Guid>(
                    filter: rt => rt.Token == refreshToken,
                    limit: 1,
                    selector: rt => new RefreshTokenInfo()
                    {
                        UserId = rt.UserId,
                        ExpiresAt = rt.ExpiresAt,
                    }
                )
            );
            var tokenInfo = tokenInfos.FirstOrDefault();
            if (tokenInfo is null)
            {
                return (false, null);
            }
            if (tokenInfo.ExpiresAt < DateTime.UtcNow)
            {
                return (false, null);
            }
            return (true, tokenInfo.UserId);
        }

        /// <summary>
        /// Delete a refresh token
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        public async Task DeleteRefreshTokenAsync(string refreshToken)
        {
            var refreshTokenUserIds = await _refreshTokenRepository.GetAllAsync<Guid, Guid>(
                filter: rt => rt.Token == refreshToken,
                limit: 1,
                selector: rt => rt.UserId
            );
            await _refreshTokenRepository.DeleteAsync(refreshTokenUserIds.FirstOrDefault());
        }
    }
}

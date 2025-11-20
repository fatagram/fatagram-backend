using System.Globalization;
using Fatagram.Application.Dtos.Token;
using Fatagram.Application.Utils;
using Fatagram.Infrastructure.Repositories;

namespace Fatagram.Application.Services.TokenServices.Interface
{
    /// <summary>
    /// Interface for the token service
    /// </summary>
    public interface ITokenService
    {
        /// <summary>
        /// Generate a new access token and refresh token
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        Task<string> GenerateAccessTokenAsync(Guid accountId);

        /// <summary>
        /// Generate a new refresh token
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        Task<string> GenerateRefreshTokenAsync(Guid accountId);

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

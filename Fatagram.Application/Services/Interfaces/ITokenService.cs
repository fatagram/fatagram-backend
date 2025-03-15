using Fatagram.Infrastructure.Repositories;
using Fatagram.Application.Utils;
using Fatagram.Application.Dtos;
using System.Globalization;

namespace Fatagram.Application.Services.Interfaces
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
        Task<Result<TokenDto>> GenerateTokensAsync(string username);


        /// <summary>
        /// Validate a refresh token
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        Task<Result<string>> ValidateRefreshToken(string refreshToken);


        /// <summary>
        /// Refresh the access token
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        Task<Result<string>> RefreshAccessTokenAsync(string refreshToken);



        /// <summary>
        /// Delete a refresh token
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        Task<Result<string>> DeleteRefreshTokenAsync(string refreshToken);
    }
}

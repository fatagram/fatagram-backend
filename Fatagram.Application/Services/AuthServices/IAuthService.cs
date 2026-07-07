using System.Security.Claims;
using Fatagram.Application.Dtos.Auth;
using Fatagram.Application.Dtos.Token;
using Fatagram.Application.Services.AuthServices.OAuth;
using Fatagram.Application.Utils;
using Fatagram.Domain.Enums;

namespace Fatagram.Application.Services.AuthServices
{
    /// <summary>
    /// Interface for the authentication service
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Login a user
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<Result<TokenDto>> Login(LoginDto loginDto);

        /// <summary>
        /// OAuth callback - authenticate user using OAuth provider
        /// </summary>
        /// <param name="provider">OAuth provider type</param>
        /// <param name="code">OAuth authorization code</param>
        /// <returns>Token response</returns>
        Task<Result<TokenDto>> OAuthCallback(
            OAuthProvider provider,
            string code,
            string? redirectUri = null
        );

        [Obsolete("Use OAuthCallback(OAuthProvider.Google, code) instead")]
        Task<Result<TokenDto>> GoogleCallback(OAuthCallbackDto request);

        /// <summary>
        /// Register a new user
        /// </summary>
        /// <param name="request"></param>
        /// /// <returns></returns>
        Task<Result> Register(RegisterDto request);

        // Task<Result<AccountsDto>> GetAccountsAsync(int page, int pageSize, string? username = null);

        /// <summary>
        /// Change the password of a user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="oldPassword"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        Task<Result<string>> ChangePasswordAsync(
            Guid userId,
            ChangePasswordDto changePasswordRequest
        );
    }
}

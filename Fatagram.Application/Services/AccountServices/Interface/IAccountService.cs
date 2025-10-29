using Fatagram.Application.Dtos.Account;
using Fatagram.Application.Dtos.Auth;
using Fatagram.Application.Utils;

namespace Fatagram.Application.Services.AccountServices.Interface
{
    /// <summary>
    /// Interface for the account service
    /// </summary>
    public interface IAccountService
    {
        /// <summary>
        /// Register a new user
        /// </summary>
        /// <param name="request"></param>
        /// /// <returns></returns>
        Task<Result<string>> Register(RegisterDto request);

        Task<Result<AccountsDto>> GetAccountsAsync(int page, int pageSize, string? username = null);

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

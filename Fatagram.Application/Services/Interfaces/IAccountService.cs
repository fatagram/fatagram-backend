using Fatagram.Application.Dtos;
using Fatagram.Application.Utils;

namespace Fatagram.Application.Services.Interfaces
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
        /// <returns></returns>
        Task<Result<string>> Register(RegisterDto request);



        /// <summary>
        /// Change the password of a user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="oldPassword"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        Task<Result<string>> ChangePasswordAsync(string userId, ChangePasswordDto changePasswordRequest);
    }
}

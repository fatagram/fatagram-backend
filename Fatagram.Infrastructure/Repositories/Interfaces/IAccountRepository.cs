using Fatagram.Domain.Models;

namespace Fatagram.Infrastructure.Repositories.Interfaces
{
    /// <summary>
    /// Interface for the account repository
    /// </summary>
    public interface IAccountRepository
    {
        /// <summary>
        /// Get an account by username
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        Task<Account?> GetAccountByUsernameAsync(string username);


        /// <summary>
        /// Get an account by account id
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        Task<Account?> GetAccountByIdAsync(Guid accountId);

        /// <summary>
        /// Get an account by account id
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        Task<Account?> GetAccountByIdAsync(string accountId);



        /// <summary>
        /// Get an account by user id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<Account?> GetAccountByUserIdAsync(Guid userId);


        /// <summary>
        /// Get an account by user id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<Account?> GetAccountByUserIdAsync(string userId);


        /// <summary>
        /// Create a new account
        /// </summary>
        /// <param name="newAccount"></param>
        /// <returns></returns>
        Task<bool> CreateAccountAsync(Account newAccount);


        /// <summary>
        /// Update an account
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        Task<bool> UpdateAccountAsync(Account account);

    }
}

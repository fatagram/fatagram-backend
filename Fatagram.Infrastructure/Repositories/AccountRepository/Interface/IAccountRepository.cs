using Fatagram.Domain.Models;

namespace Fatagram.Infrastructure.Repositories.AccountRepository.Interface
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
        Task<Account?> GetByUsernameAsync(string username);

        /// <summary>
        /// Get an account by account id
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        Task<Account?> GetAsync(Guid accountId);

        Task<(List<Account> accounts, int totalPage, int totalAccount)> GetAccountsAsync(
            int page,
            int pageSize,
            string? username = null
        );

        /// <summary>
        /// Get an account by user id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<Account?> GetByUserIdAsync(Guid userId);

        /// <summary>
        /// Create a new account
        /// </summary>
        /// <param name="newAccount"></param>
        /// <returns></returns>
        Task AddAsync(Account newAccount);

        /// <summary>
        /// Update an account
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        Task UpdateAsync(Account account);
    }
}

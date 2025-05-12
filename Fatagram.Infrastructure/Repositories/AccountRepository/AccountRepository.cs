using Fatagram.Domain.Models;
using Fatagram.Infrastructure.Data;
using Fatagram.Infrastructure.Repositories.AccountRepository.Interface;
using Fatagram.Shared.Extensions;
using Fatagram.Shared.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using System.Diagnostics;

namespace Fatagram.Infrastructure.Repositories.AccountRepository
{
    /// <summary>
    /// Repository for the account model
    /// </summary>
    public class AccountRepository : IAccountRepository
    {
        private readonly AppDbContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        public AccountRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Creates a new account asynchronously.
        /// </summary>
        /// <param name="newAccount">The new account to create.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the result of the operation.</returns>
        public async Task CreateAccountAsync(Account newAccount)
        {
            await _dbContext.Accounts.AddAsync(newAccount);
            await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Gets an account by its unique identifier asynchronously.
        /// </summary>
        /// <param name="accountId">The unique identifier of the account.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the account if found, otherwise null.</returns>
        public async Task<Account?> GetAccountByIdAsync(Guid accountId)
        {
            var account = await _dbContext.Accounts.FirstOrDefaultAsync(a => a.Id == accountId);
            return account;
        }

        /// <summary>
        /// Gets an account by its unique identifier asynchronously.
        /// </summary>
        /// <param name="accountId">The unique identifier of the account as a string.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the account if found, otherwise null.</returns>
        public async Task<Account?> GetAccountByIdAsync(string accountId)
            => await GetAccountByIdAsync(accountId.ToGuid());

        /// <summary>
        /// Gets an account by the user's unique identifier asynchronously.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the account if found, otherwise null.</returns>
        public async Task<Account?> GetAccountByUserIdAsync(Guid userId)
        {
            var account = await _dbContext.Accounts.FirstOrDefaultAsync(a => a.UserId == userId);
            return account;
        }

        /// <summary>
        /// Gets an account by the user's unique identifier asynchronously.
        /// </summary>
        /// <param name="userId">The unique identifier of the user as a string.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the account if found, otherwise null.</returns>
        public async Task<Account?> GetAccountByUserIdAsync(string userId)
            => await GetAccountByUserIdAsync(userId.ToGuid()); 

        /// <summary>
        /// Gets an account by its username asynchronously.
        /// </summary>
        /// <param name="username">The username of the account.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the account if found, otherwise null.</returns>
        public async Task<Account?> GetAccountByUsernameAsync(string username)
        {
            var account = await _dbContext.Accounts.FirstOrDefaultAsync(a => a.Username == username);
            return account;
        }

        /// <summary>
        /// Updates an account asynchronously.
        /// </summary>
        /// <param name="account">The account to update.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the result of the operation.</returns>
        public async Task UpdateAccountAsync(Account account)
        {
            _dbContext.Accounts.Update(account);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<(List<Account> accounts, int totalPage, int totalAccount)> GetAccountsAsync(int page, int pageSize, string? username = null)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 1;

            IQueryable<Account> query = _dbContext.Accounts.Include(a => a.User);

            if (!string.IsNullOrEmpty(username))
            {
                query = query.Where(a => a.Username.StartsWith(username));
            }

            var totalAccount = await query.CountAsync();
            var totalPage = (int)Math.Ceiling((double)totalAccount / pageSize);
            var accounts = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (accounts, totalPage, totalAccount);
        }
    }
}

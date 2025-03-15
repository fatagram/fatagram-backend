using Fatagram.Domain.Models;
using Fatagram.Infrastructure.Data;
using Fatagram.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using System.Diagnostics;

namespace Fatagram.Infrastructure.Repositories
{
    /// <summary>
    /// Repository for the account model
    /// </summary>
    public class AccountRepository : IAccountRepository
    {
        private readonly AppDbContext _dbContext;

        public AccountRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        /// <summary>
        /// Get an account by username
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public async Task<Account?> GetAccountByUsernameAsync(string username)
        {
            /*
             * Collate makes the query case sensitive
             */
            var account = await _dbContext.Accounts.Where(x => EF.Functions.Collate(x.Username, "SQL_Latin1_General_CP1_CS_AS") == username)
                .FirstOrDefaultAsync();

            return account;
        }



        /// <summary>
        /// Get an account by account id
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public async Task<Account?> GetAccountByIdAsync(Guid accountId)
        {
            return await _dbContext.Accounts.FirstOrDefaultAsync(x => x.Id == accountId);
        }


        /// <summary>
        /// Get an account by account id
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public async Task<Account?> GetAccountByIdAsync(string accountId) => await GetAccountByIdAsync(Guid.Parse(accountId));



        /// <summary>
        /// Get an account by account id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<Account?> GetAccountByUserIdAsync(Guid userId)
        {
            throw new System.NotImplementedException();
        }


        /// <summary>
        /// Get an account by account id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<Account?> GetAccountByUserIdAsync(string userId) => await GetAccountByUserIdAsync(userId);



        /// <summary>
        /// Create a new account
        /// </summary>
        /// <param name="newAccount"></param>
        /// <returns></returns>
        public async Task<bool> CreateAccountAsync(Account newAccount)
        {       
            await _dbContext.Accounts.AddAsync(newAccount);
            await _dbContext.SaveChangesAsync();
            return true;
        }



        /// <summary>
        /// Update an account
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public async Task<bool> UpdateAccountAsync(Account account)
        {
            _dbContext.Accounts.Update(account);
            await _dbContext.SaveChangesAsync();
            return true;
        }



    }
}

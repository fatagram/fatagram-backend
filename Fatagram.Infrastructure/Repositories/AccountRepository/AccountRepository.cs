using Fatagram.Domain.Models;
using Fatagram.Infrastructure.Data;
using Fatagram.Infrastructure.Repositories.AccountRepository.Interface;
using Fatagram.Infrastructure.Repositories.BaseRepository;
using Microsoft.EntityFrameworkCore;

namespace Fatagram.Infrastructure.Repositories.AccountRepository
{
    /// <summary>
    /// Repository for the account model
    /// </summary>
    public class AccountRepository : BaseRepository<Account>, IAccountRepository
    {
        public AccountRepository(AppDbContext dbContext)
            : base(dbContext) { }

        public async Task<Account?> GetByEmailAsync(string email, bool? isVerified)
        {
            return await _dbSet
                .Join(
                    this._dbContext.Emails,
                    a => a.Id,
                    e => e.AccountId,
                    (a, e) => new { Account = a, Email = e }
                )
                .Where(joined =>
                    joined.Email.Address == email
                    && (isVerified == null || joined.Email.IsVerified == isVerified.Value)
                )
                .Select(joined => joined.Account)
                .FirstOrDefaultAsync();
        }
    }
}

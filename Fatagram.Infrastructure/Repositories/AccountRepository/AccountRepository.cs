using Fatagram.Domain.Models;
using Fatagram.Infrastructure.Data;
using Fatagram.Infrastructure.Repositories.AccountRepository.Interface;
using Fatagram.Infrastructure.Repositories.BaseRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Fatagram.Infrastructure.Repositories.AccountRepository
{
    /// <summary>
    /// Repository for the account model
    /// </summary>
    public class AccountRepository(
        AppDbContext dbContext,
        ILogger<AccountRepository>? logger = null
    ) : BaseRepository<Account>(dbContext, logger), IAccountRepository
    {
        public async Task<Account?> GetByEmailAsync(string email)
        {
            return await _dbSet
                .Join(
                    _dbContext.Emails,
                    a => a.Id,
                    e => e.AccountId,
                    (a, e) => new { Account = a, Email = e }
                )
                .Where(joined => joined.Email.Address == email)
                .Select(joined => joined.Account)
                .FirstOrDefaultAsync();
        }
    }
}

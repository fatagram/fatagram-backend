using Fatagram.Domain.Models;
using Fatagram.Infrastructure.Data;
using Fatagram.Application.Abstractions.Repositories;
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
        public async Task<Account?> GetByUsernameOrEmailAsync(string usernameOrEmail)
        {
            return await _dbSet
                .Where(a =>
                    a.Username == usernameOrEmail
                    || a.User.UserEmails.Any(ue =>
                        ue.Email.Address == usernameOrEmail && ue.IsVerified
                    )
                )
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }
    }
}

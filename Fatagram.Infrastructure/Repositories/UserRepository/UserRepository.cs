using System.Diagnostics.Metrics;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using Fatagram.Domain.Models;
using Fatagram.Infrastructure.Data;
using Fatagram.Infrastructure.Repositories.BaseRepository;
using Fatagram.Infrastructure.Repositories.UserRepository.Interface;
using Fatagram.Shared.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Fatagram.Infrastructure.Repositories.UserRepository
{
    /// <summary>
    /// User repository
    /// </summary>
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        public UserRepository(AppDbContext dbContext)
            : base(dbContext) { }

        public Task<User?> GetByUsernameAsync(string username)
        {
            return _dbSet
                .Join(
                    this._dbContext.Accounts,
                    user => user.Id,
                    account => account.UserId,
                    (user, account) => new { User = user, Account = account }
                )
                .Where(ua => ua.Account.Username == username)
                .Select(ua => ua.User)
                .FirstOrDefaultAsync();
        }
    }
}

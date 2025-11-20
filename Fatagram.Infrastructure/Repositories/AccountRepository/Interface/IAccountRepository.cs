using Fatagram.Domain.Models;
using Fatagram.Infrastructure.Repositories.BaseRepository.Interfaces;

namespace Fatagram.Infrastructure.Repositories.AccountRepository.Interface
{
    /// <summary>
    /// Interface for the account repository
    /// </summary>
    public interface IAccountRepository : IBaseRepository<Account>
    {
        Task<Account?> GetByEmailAsync(string email, bool? isVerified = null);
    }
}

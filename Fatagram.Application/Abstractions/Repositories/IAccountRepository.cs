using System.Threading.Tasks;
using Fatagram.Domain.Models;

namespace Fatagram.Application.Abstractions.Repositories
{
    /// <summary>
    /// Interface for the account repository
    /// </summary>
    public interface IAccountRepository : IBaseRepository<Account>
    {
        Task<Account?> GetByUsernameOrEmailAsync(string usernameOrEmail);
    }
}

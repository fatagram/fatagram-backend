using System.Threading.Tasks;
using Fatagram.Domain.Models;

namespace Fatagram.Application.Abstractions.Repositories
{
    /// <summary>
    /// Interface for the user repository
    /// </summary>
    public interface IUserRepository : IBaseRepository<User>
    {
        Task<User?> GetByUsernameAsync(string username);
    }
}

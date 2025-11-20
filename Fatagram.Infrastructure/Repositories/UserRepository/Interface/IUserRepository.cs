using Fatagram.Domain.Models;
using Fatagram.Infrastructure.Repositories.BaseRepository.Interfaces;

namespace Fatagram.Infrastructure.Repositories.UserRepository.Interface
{
    /// <summary>
    /// Interface for the user repository
    /// </summary>
    public interface IUserRepository : IBaseRepository<User>
    {
        Task<User?> GetByUsernameAsync(string username);
    }
}

using Fatagram.Domain.Models;
using Fatagram.Shared.Utils;

namespace Fatagram.Infrastructure.Repositories.UserRepository.Interface
{
    /// <summary>
    /// Interface for the user repository
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Get a user by username
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        Task<User?> GetUserByUsernameAsync(string username);

        /// <summary>
        /// Get a user by url name
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        Task<User?> GetUserByUrlNameAsync(string username);
            
        /// <summary>
        /// Get a user by user id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<User?> GetUser(string key);

        /// <summary>
        /// Create a new user
        /// </summary>
        /// <param name="newUser"></param>
        /// <returns></returns>
        Task CreateUserAsync(User newUser);

        /// <summary>
        /// Update a user
        /// </summary>
        /// <param name="updateUser"></param>
        /// <returns></returns>
        Task UpdateUserAsync(User updateUser);


    }
}

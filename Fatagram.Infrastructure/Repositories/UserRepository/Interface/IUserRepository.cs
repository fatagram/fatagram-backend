using Fatagram.Domain.Models;

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
        Task<User?> GetByUsernameAsync(string username);

        /// <summary>
        /// Get a user by url name
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        Task<User?> GetByUrlNameAsync(string username);

        /// <summary>
        /// Get a user by email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        Task<User?> GetByEmailAsync(string email);
            
        /// <summary>
        /// Get a user by user id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<User?> GetAsync(string key);

        /// <summary>
        /// Create a new user
        /// </summary>
        /// <param name="newUser"></param>
        /// <returns></returns>
        Task AddAsync(User newUser);

        /// <summary>
        /// Update a user
        /// </summary>
        /// <param name="updateUser"></param>
        /// <returns></returns>
        Task UpdateAsync(User updateUser);

    }
}

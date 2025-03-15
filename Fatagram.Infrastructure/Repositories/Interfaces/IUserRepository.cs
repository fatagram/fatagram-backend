using Fatagram.Domain.Models;

namespace Fatagram.Infrastructure.Repositories.Interfaces
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
        /// Get a user by user id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<User?> GetUserByIdAsync(Guid id);

        /// <summary>
        /// Get a user by user id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<User?> GetUserByIdAsync(string id);

        /// <summary>
        /// Create a new user
        /// </summary>
        /// <param name="newUser"></param>
        /// <returns></returns>
        Task<bool> CreateUserAsync(User newUser);

        /// <summary>
        /// Update a user
        /// </summary>
        /// <param name="updateUser"></param>
        /// <returns></returns>
        Task<bool> UpdateUserAsync(User updateUser);



        /// <summary>
        /// Get a user's info with field
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        Task<string> GetUserInfoByField(Guid userId, string field);


        /// <summary>
        /// Get a user's info with field
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        Task<string> GetUserInfoByField(string userId, string field);


        /// <summary>
        /// Get a user's infos with fields
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="fields"></param>
        /// <param name="applyPrivacy"></param>
        /// <returns></returns>
        Task<string?[]> GetUserInfosWithFieldsAsync(string userId, string[] fields);

    }
}

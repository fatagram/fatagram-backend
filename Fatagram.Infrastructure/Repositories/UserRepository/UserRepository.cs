using Fatagram.Domain.Models;
using Fatagram.Infrastructure.Data;
using Fatagram.Infrastructure.Repositories.UserRepository.Interface;
using Fatagram.Shared.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Fatagram.Infrastructure.Repositories.UserRepository
{
    /// <summary>
    /// User repository
    /// </summary>
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        public UserRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Creates a new user asynchronously.
        /// </summary>
        /// <param name="newUser">The new user to create.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the result of the operation.</returns>
        public async Task CreateUserAsync(User newUser)
        {
            await _dbContext.Users.AddAsync(newUser);
            await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Gets a user by their unique identifier asynchronously.
        /// </summary>
        /// <param name="id">The unique identifier of the user.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the user if found, otherwise null.</returns>
        public async Task<User?> GetUser(string key)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == key.ToGuid() || u.Username == key);
            return user;
        }

        ///// <summary>
        ///// Gets a user by their unique identifier asynchronously.
        ///// </summary>
        ///// <param name="id">The unique identifier of the user as a string.</param>
        ///// <returns>A task that represents the asynchronous operation. The task result contains the user if found, otherwise null.</returns>
        //public async Task<User?> GetUserByIdAsync(string id)
        //    => await GetUserByIdAsync(id.ToGuid());

        /// <summary>
        /// Gets a user by their username asynchronously.
        /// </summary>
        /// <param name="username">The username of the user.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the user if found, otherwise null.</returns>
        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            var userId = await _dbContext.Accounts
                    .Where(a => a.Username == username)
                    .Select(a => a.UserId)
                    .FirstOrDefaultAsync();

            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
            return user;
        }

        /// <summary>
        /// Updates a user asynchronously.
        /// </summary>
        /// <param name="updateUser">The user to update.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the result of the operation.</returns>
        public async Task UpdateUserAsync(User updateUser)
        {
            _dbContext.Users.Update(updateUser);
            await _dbContext.SaveChangesAsync();
        }
    }
}

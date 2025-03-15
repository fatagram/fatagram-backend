using Fatagram.Domain.Models;
using Fatagram.Infrastructure.Data;
using Fatagram.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Fatagram.Infrastructure.Repositories.MockDB
{
    /// <summary>
    /// User repository
    /// </summary>
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _dbContext;

        public UserRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Create a new user
        /// </summary>
        /// <param name="newUser"></param>
        /// <returns></returns>
        public async Task<bool> CreateUserAsync(User newUser)
        {
            await _dbContext.Users.AddAsync(newUser);
            await _dbContext.SaveChangesAsync();
            return true;
        }


        /// <summary>
        /// Get a user by username
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            var account = await _dbContext.Accounts.FirstOrDefaultAsync(x => x.Username == username);

            if (account == null) return null;

            return await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == account.UserId); ;
        }



        /// <summary>
        /// Get a user by user id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<User?> GetUserByIdAsync(Guid id)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == id); ;
        }



        /// <summary>
        /// Update a user
        /// </summary>
        /// <param name="updateUser"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task<bool> UpdateUserAsync(User updateUser)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Get a user by user id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<User?> GetUserByIdAsync(string id) => GetUserByIdAsync(Guid.Parse(id));


        /// <summary>
        /// Get a user by username
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task<string[]> GetUserInfosWithFieldsAsync(string userId, string[] fields)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetUserInfoByField(Guid userId, string field)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetUserInfoByField(string userId, string field)
        {
            throw new NotImplementedException();
        }
    }
}

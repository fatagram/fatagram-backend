using Fatagram.Application.Utils;

namespace Fatagram.Application.Services.Interfaces
{
    public interface IProfileService
    {
        /// <summary>
        /// Get a user by username
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<Result<string>> GetUserInfoByFieldsAsync(string userId, string fields);


        /// <summary>
        /// Get a user by id
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        Task<Result<string>> GetUserInfoByFieldsAsync(Guid userId, string fields);


        /// <summary>
        /// Get all user info
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<Result<string>> GetAllUserInfoAsync(string userId);


        /// <summary>
        /// Get all user info
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<Result<string>> GetAllUserInfoAsync(Guid userId);


    }
}

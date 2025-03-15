using Fatagram.Domain.Enums;
using Fatagram.Application.Utils;
using Fatagram.Application.Dtos;

namespace Fatagram.Application.Services.Interfaces
{
    /// <summary>
    /// Interface for the user service
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Update a user info
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<Result<string>> UpdateUserAsync(string userId, UpdateUserDto request);



        /// <summary>
        /// Update a user info
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<Result<string>> UpdateUserAsync(Guid userId, UpdateUserDto request);



        /// <summary>
        /// Get a user by username
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        Task<Result<Dictionary<string, string>>> GetUserInfoByFieldsAsync(string userId, string fields);


        /// <summary>
        /// Get a user by id
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        Task<Result<Dictionary<string, string>>> GetUserInfoByFieldsAsync(Guid userId, string fields);

    }
}

using Fatagram.Application.Dtos;
using Fatagram.Application.Utils;

namespace Fatagram.Application.Services.Interfaces
{
    public interface IOtherUserService
    {

        /// <summary>
        /// Get a user by id
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        Task<Result<UserDto>> GetOtherUserInfoByFieldsAsync(Guid userId, string fields);

        /// <summary>
        /// Get a user by username
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        Task<Result<UserDto>> GetOtherUserInfoByFieldsAsync(string userId, string fields);


    }
}

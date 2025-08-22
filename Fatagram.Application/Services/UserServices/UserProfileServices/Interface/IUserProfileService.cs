using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.Application.Dtos.User;
using Fatagram.Application.Dtos.User.Update;
using Fatagram.Application.Utils;

namespace Fatagram.Application.Services.UserServices.UserProfileServices.Interface
{
    public interface IUserProfileService
    {
        /// <summary>
        /// Update a user info
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<Result<UpdateUserDto>> UpdateUserAsync(string userId, UpdateUserDto request);


        /// <summary>
        /// Update a user info
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<Result<UpdateUserDto>> UpdateUserAsync(Guid userId, UpdateUserDto request);


        /// <summary>
        /// Update a url name
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="changeUrlNameDto"></param>
        /// <returns></returns>
        Task<Result<ChangeUrlNameDto>> UpdateUrlNameAsync(Guid userId, ChangeUrlNameDto changeUrlNameDto);

        /// <summary>
        /// Update a url name
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="changeUrlNameDto"></param>
        /// <returns></returns>
        Task<Result<ChangeUrlNameDto>> UpdateUrlNameAsync(string userId, ChangeUrlNameDto changeUrlNameDto);

        /// <summary>
        /// Update a name
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="changeNameDto"></param>
        /// <returns></returns>
        Task<Result<ChangeNameDto>> UpdateNameAsync(string userId, ChangeNameDto changeNameDto);


        /// <summary>
        /// Get a user by username
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        Task<Result<GetUserProfileDto>> GetUserInfoAuthenticatedAsync(string senderId, string targetId, string fields);


        /// <summary>
        /// Get a user by id
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        Task<Result<GetUserProfileDto>> GetUserInfoAuthenticatedAsync(Guid userId, Guid targetId, string fields);

        /// <summary>
        /// Get a user by username
        /// </summary>
        /// <param name="targetId"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        Task<Result<GetUserProfileDto>> GetUserInfoPublicAsync(string targetId, string fields);

        /// <summary>
        /// Get a user by id
        /// </summary>
        /// <param name="targetId"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        Task<Result<GetUserProfileDto>> GetUserInfoPublicAsync(Guid targetId, string fields);

        /// <summary>
        /// Check if a user exists
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<Result<string>> CheckUserExistAsync(string userId);
        Task<Result<string>> CheckUserExistAsync(Guid userId);
    }
}
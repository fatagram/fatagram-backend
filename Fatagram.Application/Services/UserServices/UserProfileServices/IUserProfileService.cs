using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.Application.Dtos.User;
using Fatagram.Application.Dtos.User.Update;
using Fatagram.Application.Utils;

namespace Fatagram.Application.Services.UserServices.UserProfileServices
{
    public interface IUserProfileService
    {
        /// <summary>
        /// Update a user info
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<Result<UpdateUserDto>> UpdateUserAsync(Guid userId, UpdateUserDto request);

        /// <summary>
        /// Update a url name
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="changeUrlNameDto"></param>
        /// <returns></returns>
        Task<Result<ChangeUrlNameDto>> UpdateUrlNameAsync(
            Guid userId,
            ChangeUrlNameDto changeUrlNameDto
        );

        Task<Result<ChangeNicknameDto>> UpdateNicknameAsync(
            Guid userId,
            ChangeNicknameDto changeNicknameDto
        );

        /// <summary>
        /// Update a name
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="changeNameDto"></param>
        /// <returns></returns>
        Task<Result<ChangeNameDto>> UpdateNameAsync(Guid userId, ChangeNameDto changeNameDto);

        /// <summary>
        /// Get a user by id
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        Task<Result<GetUserProfileDto>> GetUserProfileAsync(
            Guid userId,
            string target,
            string fields
        );

        /// <summary>
        /// Check if a user exists
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<Result<string>> CheckUserExistAsync(Guid userId);

        /// <summary>
        /// Check if onboarding is completed
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<Result<bool>> IsOnboardingCompletedAsync(Guid userId);

        /// <summary>
        /// Update onboarding
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="onboardingDto"></param>
        /// <returns></returns>
        Task<Result> OnboardingAsync(Guid userId, OnboardingDto onboardingDto);

        /// <summary>
        /// Get default data for onboarding
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<Result<OnboardingDefaultDataDto>> GetOnboardingDefaultDataAsync(Guid userId);
    }
}

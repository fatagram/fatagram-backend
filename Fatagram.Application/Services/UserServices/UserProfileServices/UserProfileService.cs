using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using Fatagram.Application.Dtos.User;
using Fatagram.Application.Dtos.User.Update;
using Fatagram.Application.Exceptions;
using Fatagram.Application.Exceptions.DetailExceptions;
using Fatagram.Application.Exceptions.MiddleLevelExceptions;
using Fatagram.Application.Services.UserServices.UserProfileServices.Interfaces;
using Fatagram.Application.Utils;
using Fatagram.Domain.Enums;
using Fatagram.Domain.Models;
using Fatagram.Infrastructure.Repositories.UserRepository.Interface;
using Fatagram.Shared.Common;
using Fatagram.Shared.Enums;
using Fatagram.Shared.Extensions;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Fatagram.Application.Services.UserServices.UserProfileServices
{
    public class UserProfileService(
        IUserRepository userRepository,
        IMapper mapper,
        ILogger<UserProfileService> logger
    ) : IUserProfileService
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<UserProfileService> _logger = logger;

        public async Task<Result<string>> CheckUserExistAsync(Guid userId)
        {
            var user =
                await _userRepository.GetAsync(userId, s => s) ?? throw new UserNotFoundException();
            return Result<string>.Create();
        }

        /// <summary>
        /// Get user info by fields
        /// </summary>
        /// <param name="targetId"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public async Task<Result<GetUserProfileDto>> GetUserProfileAsync(
            Guid userId,
            string target,
            string fields
        )
        {
            var user =
                await _userRepository.GetDynamicAsync(
                    fields,
                    u => u.Id.ToString() == target || u.UrlName == target
                ) ?? throw new UserNotFoundException();

            // Map dynamic object to dictionary
            var infos = new Dictionary<string, string?>();

            // Convert dynamic to dictionary safely
            if (user != null)
            {
                var userObj = (object)user;
                var properties = userObj.GetType().GetProperties();

                foreach (var prop in properties)
                {
                    try
                    {
                        var value = prop.GetValue(userObj, null);
                        infos[prop.Name.ToLowerFirstLetter()] = value?.ToString();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to get property {PropertyName}", prop.Name);
                        infos[prop.Name.ToLowerFirstLetter()] = null;
                    }
                }
            }

            var result = new GetUserProfileDto
            {
                Infos = infos,
                IsOwner = userId.ToString() == target,
            };

            return Result<GetUserProfileDto>.Create(ResponseStatusCode.Success, result);
        }

        /// <summary>
        /// Update a user info
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="updateUserDto"></param>
        /// <returns></returns>
        public async Task<Result<UpdateUserDto>> UpdateUserAsync(
            Guid userId,
            UpdateUserDto updateUserDto
        )
        {
            var existingUser =
                await _userRepository.GetAsync(userId, u => u) ?? throw new UserNotFoundException();

            _mapper.Map(updateUserDto, existingUser);
            await _userRepository.UpdateAsync(existingUser.NormalizeEmptyStringToNull());
            return Result<UpdateUserDto>.Create(ResponseStatusCode.Success, updateUserDto);
        }

        public async Task<Result<ChangeUrlNameDto>> UpdateUrlNameAsync(
            Guid userId,
            ChangeUrlNameDto changeUrlNameDto
        )
        {
            var user =
                await _userRepository.GetAsync(userId, u => u) ?? throw new UserNotFoundException();

            var _user = await _userRepository.GetAllAsync<Account, Guid>(
                filter: u => u.UrlName == changeUrlNameDto.UrlName,
                limit: 1
            );
            var existUser = _user.FirstOrDefault();
            if (existUser != null && existUser.Id != user.Id)
            {
                throw new BadRequestException(
                    new Error("URLNAME_ALREADY_EXISTS", "Url name already exists.")
                );
            }
            user.UrlName = changeUrlNameDto.UrlName;
            await _userRepository.UpdateAsync(user);
            return Result<ChangeUrlNameDto>.Create(ResponseStatusCode.Success, changeUrlNameDto);
        }

        public async Task<Result<ChangeNameDto>> UpdateNameAsync(
            Guid userId,
            ChangeNameDto changeNameDto
        )
        {
            var user =
                await _userRepository.GetAsync(userId, u => u) ?? throw new UserNotFoundException();
            user.FirstName = changeNameDto.FirstName;
            user.MiddleName = changeNameDto.MiddleName;
            user.LastName = changeNameDto.LastName;

            // Build full name with optional middle name
            user.FullName = string.IsNullOrWhiteSpace(changeNameDto.MiddleName)
                ? $"{changeNameDto.FirstName} {changeNameDto.LastName}"
                : $"{changeNameDto.FirstName} {changeNameDto.MiddleName} {changeNameDto.LastName}";

            await _userRepository.UpdateAsync(user);
            return Result<ChangeNameDto>.Create(ResponseStatusCode.Success, changeNameDto);
        }

        public async Task<Result<bool>> IsOnboardingCompletedAsync(Guid userId)
        {
            _logger.LogInformation("Checking onboarding status for user {UserId}", userId);
            var isOnboarding = await _userRepository.GetAsync(userId, u => u.IsOnBoarding);
            return Result<bool>.Create(ResponseStatusCode.Success, isOnboarding);
        }

        public async Task<Result> OnboardingAsync(Guid userId, OnboardingDto onboardingDto)
        {
            var result = await _userRepository.UpdateAsync(
                u => u.Id == userId,
                u =>
                {
                    u.IsOnBoarding = true;
                    u.BirthDay = onboardingDto.BirthDay;
                    u.Gender = onboardingDto.Gender;
                    u.FirstName = onboardingDto.FirstName;
                    u.MiddleName = onboardingDto.MiddleName;
                    u.LastName = onboardingDto.LastName;
                    u.FullName =
                        onboardingDto.FirstName
                        + " "
                        + onboardingDto.MiddleName
                        + " "
                        + onboardingDto.LastName;
                }
            );
            return Result.Create(ResponseStatusCode.Created);
        }

        public async Task<Result<OnboardingDefaultDataDto>> GetOnboardingDefaultDataAsync(
            Guid userId
        )
        {
            var user =
                await _userRepository.GetAsync(
                    userId,
                    u => new
                    {
                        u.FirstName,
                        u.LastName,
                        u.MiddleName,
                        u.Avatar,
                        u.Gender,
                        u.BirthDay,
                        Email = u
                            .Emails
                            .Where(e => e.IsPrimary)
                            .Select(e => e.Address)
                            .FirstOrDefault(),
                    }
                ) ?? throw new UserNotFoundException();

            var data = new OnboardingDefaultDataDto
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                MiddleName = user.MiddleName,
                Avatar = user.Avatar,
                Gender = user.Gender,
                BirthDay = user.BirthDay,
                Email = user.Email,
            };
            return Result<OnboardingDefaultDataDto>.Create(ResponseStatusCode.Success, data);
        }

        public async Task<Result<List<EmailDto>>> GetUserEmailsAsync(Guid userId)
        {            var user = await _userRepository.GetAllAsync<User, Guid>(
                filter: u => u.Id == userId,
                include: q => q.Include(u => u.Emails)
            );

            var emails = user.FirstOrDefault()?.Emails
                .Select(e => new EmailDto
                {
                    Id = e.Id,
                    Address = e.Address,
                    IsPrimary = e.IsPrimary,
                    IsVerified = e.IsVerified
                })
                .ToList() ?? new List<EmailDto>();

            return Result<List<EmailDto>>.Create(ResponseStatusCode.Success, emails);
        }

        public async Task<Result<PhoneDto>> GetUserPhoneAsync(Guid userId)
        {
            var user = await _userRepository.GetAsync(
                userId,
                u => new PhoneDto { Phone = u.Phone }
            ) ?? throw new UserNotFoundException();

            return Result<PhoneDto>.Create(ResponseStatusCode.Success, user);
        }
    }
}

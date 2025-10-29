using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Fatagram.Application.Dtos.User;
using Fatagram.Application.Dtos.User.Update;
using Fatagram.Application.Exceptions;
using Fatagram.Application.Services.UserServices.UserProfileServices.Interface;
using Fatagram.Application.Utils;
using Fatagram.Application.Validation;
using Fatagram.Domain.Enums;
using Fatagram.Infrastructure.Repositories.UserPrivacyRepository.Interface;
using Fatagram.Infrastructure.Repositories.UserRepository.Interface;
using Fatagram.Shared.Enums;
using Fatagram.Shared.Extensions;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace Fatagram.Application.Services.UserServices.UserProfileServices
{
    public class UserProfileService : IUserProfileService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserPrivacyRepository _userPrivacyRepository;
        private readonly IMapper _mapper;

        public UserProfileService(
            IUserRepository userRepository,
            IUserPrivacyRepository userPrivacyRepository,
            IMapper mapper
        )
        {
            _userRepository = userRepository;
            _userPrivacyRepository = userPrivacyRepository;
            _mapper = mapper;
        }

        public async Task<Result<string>> CheckUserExistAsync(Guid userId)
        {
            var user = await _userRepository.GetAsync(userId.ToString());
            if (user is null)
            {
                throw new UserNotFoundException();
            }
            return Result<string>.Success();
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
            if (string.IsNullOrEmpty(fields))
            {
                return Result<GetUserProfileDto>.Failure(
                    ResponseStatusCode.BadRequest,
                    "FIELDS_REQUIRED",
                    "Fields are required"
                );
            }
            var listField = fields.Split(',').ToList();
            listField.Add("id");

            var res = await _userRepository.GetAsync(target, listField);
            var isOwner = userId.ToString() == res["id"]?.ToString();
            listField.RemoveAt(listField.Count - 1);

            var privacyDict = res.ToDictionary(k => k.Key, v => v.Value?.ToString());

            return Result<GetUserProfileDto>.Success(
                ResponseStatusCode.Success,
                new GetUserProfileDto() { Infos = privacyDict, IsOwner = isOwner }
            );
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
            var existingUser = await _userRepository.GetAsync(userId.ToString());
            if (existingUser is null)
                throw new UserNotFoundException();

            _mapper.Map(updateUserDto, existingUser);
            await _userRepository.UpdateAsync(existingUser.NormalizeEmptyStringToNull());
            return Result<UpdateUserDto>.Success(ResponseStatusCode.Success, updateUserDto);
        }

        public async Task<Result<ChangeUrlNameDto>> UpdateUrlNameAsync(
            Guid userId,
            ChangeUrlNameDto changeUrlNameDto
        )
        {
            ValidationHelper.EnsureValidUrlName(changeUrlNameDto.UrlName);
            var user = await _userRepository.GetAsync(userId.ToString());
            if (user == null)
                throw new UserNotFoundException();

            var existUser = await _userRepository.GetAsync(changeUrlNameDto.UrlName);
            if (existUser != null && existUser.Id != user.Id)
            {
                return Result<ChangeUrlNameDto>.Failure(
                    ResponseStatusCode.BadRequest,
                    "USERNAME_EXISTED",
                    "Username existed"
                );
            }
            user.UrlName = changeUrlNameDto.UrlName;
            await _userRepository.UpdateAsync(user);
            return Result<ChangeUrlNameDto>.Success(ResponseStatusCode.Success, changeUrlNameDto);
        }

        public async Task<Result<ChangeNameDto>> UpdateNameAsync(
            Guid userId,
            ChangeNameDto changeNameDto
        )
        {
            var user = await _userRepository.GetAsync(userId.ToString());
            if (user == null)
            {
                throw new UserNotFoundException();
            }
            user.FirstName = changeNameDto.FirstName;
            user.LastName = changeNameDto.LastName;
            user.FullName = $"{changeNameDto.FirstName} {changeNameDto.LastName}";
            await _userRepository.UpdateAsync(user);
            return Result<ChangeNameDto>.Success(ResponseStatusCode.Success, changeNameDto);
        }
    }
}

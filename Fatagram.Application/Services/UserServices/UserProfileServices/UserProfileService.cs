using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Fatagram.Application.Dtos.User;
using Fatagram.Application.Dtos.User.Update;
using Fatagram.Application.Exceptions;
using Fatagram.Application.Services.UserServices.UserProfileServices.Interfaces;
using Fatagram.Application.Utils;
using Fatagram.Domain.Enums;
using Fatagram.Infrastructure.Repositories.UserRepository.Interface;
using Fatagram.Shared.Enums;
using Fatagram.Shared.Extensions;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace Fatagram.Application.Services.UserServices.UserProfileServices
{
    public class UserProfileService : IUserProfileService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserProfileService(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<Result<string>> CheckUserExistAsync(Guid userId)
        {
            var user = await _userRepository.GetAsync(userId, s => s);
            if (user is null)
            {
                throw new UserNotFoundException();
            }
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
            var user = await _userRepository.GetDynamicAsync(
                fields,
                u => u.Id.ToString() == target
            );
            if (user is null)
            {
                throw new UserNotFoundException();
            }

            // Map dynamic object to dictionary
            var infos = new Dictionary<string, string?>();
            var properties = ((object)user).GetType().GetProperties();

            foreach (var prop in properties)
            {
                var value = prop.GetValue(user);
                infos[prop.Name] = value?.ToString();
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
            var existingUser = await _userRepository.GetAsync(userId, u => u);
            if (existingUser is null)
                throw new UserNotFoundException();

            _mapper.Map(updateUserDto, existingUser);
            await _userRepository.UpdateAsync(existingUser.NormalizeEmptyStringToNull());
            return Result<UpdateUserDto>.Create(ResponseStatusCode.Success, updateUserDto);
        }

        public async Task<Result<ChangeUrlNameDto>> UpdateUrlNameAsync(
            Guid userId,
            ChangeUrlNameDto changeUrlNameDto
        )
        {
            var user = await _userRepository.GetAsync(userId, u => u);
            if (user == null)
                throw new UserNotFoundException();

            var existUser = await _userRepository.GetByUniqueKeyAsync(
                u => u.UrlName,
                changeUrlNameDto.UrlName,
                u => u
            );
            if (existUser != null && existUser.Id != user.Id)
            {
                throw new AppException("URLNAME_ALREADY_EXISTS", "Url name already exists.");
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
            var user = await _userRepository.GetAsync(userId, u => u);
            if (user == null)
            {
                throw new UserNotFoundException();
            }
            user.FirstName = changeNameDto.FirstName;
            user.LastName = changeNameDto.LastName;
            user.FullName = $"{changeNameDto.FirstName} {changeNameDto.LastName}";
            await _userRepository.UpdateAsync(user);
            return Result<ChangeNameDto>.Create(ResponseStatusCode.Success, changeNameDto);
        }
    }
}

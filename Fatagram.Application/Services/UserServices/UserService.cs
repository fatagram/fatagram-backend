using AutoMapper;
using Fatagram.Application.Checker;
using Fatagram.Application.Dtos.User;
using Fatagram.Application.Dtos.User.Update;
using Fatagram.Application.Exceptions;
using Fatagram.Application.Services.UserServices.Interface;
using Fatagram.Application.Utils;
using Fatagram.Application.Validation;
using Fatagram.Domain.Enums;
using Fatagram.Domain.Models;
using Fatagram.Domain.Utils;
using Fatagram.Infrastructure.Repositories.AccountRepository.Interface;
using Fatagram.Infrastructure.Repositories.UserPrivacyRepository.Interface;
using Fatagram.Infrastructure.Repositories.UserRepository.Interface;
using Fatagram.Shared.Extensions;
using Fatagram.Shared.Utils;
using Microsoft.AspNetCore.Http;

namespace Fatagram.Application.Services.UserServices
{
    /// <summary>
    /// User service
    /// </summary>
    public class UserService : IUserService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUserPrivacyRepository _userPrivacyRepository;
        private readonly IMapper _mapper;
        private readonly UserChecker _userChecker;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private List<string> _canNotUpdateProperties = new List<string>()
        {
            nameof(User.Id),
        };

        public UserService(IAccountRepository accountRepository,
            IUserRepository userRepository, 
            IUserPrivacyRepository userPrivacyRepository, 
            IMapper mapper,
            UserChecker userChecker,
            IHttpContextAccessor httpContextAccessor)
        {
            _accountRepository = accountRepository;
            _userRepository = userRepository;
            _userPrivacyRepository = userPrivacyRepository;
            _mapper = mapper;
            _userChecker = userChecker;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Result<string>> CheckUserExistAsync(string userId)
        {
            var user = await _userRepository.GetUser(userId);
            if (user is null) throw new UserNotFoundException();
            return Result<string>.Success();
        }

        public Task<Result<string>> CheckUserExistAsync(Guid userId)
            => CheckUserExistAsync(userId.ToString());


        /// <summary>
        /// Get user info by fields
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public async Task<Result<GetUserProfileDto>> GetUserInfoAuthenticatedAsync(string senderId, string target, string fields)
        {
            var res = await _userRepository.GetUser(target);
            if (res is null) throw new UserNotFoundException();

            var privacyDict = new Dictionary<string, string?>();
            var listField = fields.Split(',').ToList();
            foreach (var field in listField)
            {
                var value = res.GetPropertyValue(field);
                if (value is not null)
                {
                    privacyDict.Add(field, value?.ToString());
                }
            }
            var targetId = res.Id.ToString();
            if (senderId == targetId)
            {
                return Result<GetUserProfileDto>.Success(new GetUserProfileDto()
                {
                    Infos = privacyDict,
                    IsOwner = true
                });
            }
            var privacyLevels = await _userPrivacyRepository.GetPrivacyLevelsAsync(targetId, listField);

            foreach (var field in listField)
            {
                privacyDict[field] = privacyLevels[field] switch
                {
                    PrivacyLevel.Public => privacyDict[field],
                    PrivacyLevel.Private => null,
                    _ => null
                };
            }

            return Result<GetUserProfileDto>.Success(new GetUserProfileDto()
            {
                Infos = privacyDict,
                IsOwner = false
            });
        }

        /// <summary>
        /// Get user info by fields
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public async Task<Result<GetUserProfileDto>> GetUserInfoAuthenticatedAsync(Guid senderId, Guid targetId, string fields)
            => await GetUserInfoAuthenticatedAsync(senderId.ToString(), targetId.ToString(), fields);

        /// <summary>
        /// Get user info by fields
        /// </summary>
        /// <param name="targetId"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public async Task<Result<GetUserProfileDto>> GetUserInfoPublicAsync(string target, string fields)
        {
            if (string.IsNullOrEmpty(fields)) throw new AppException("FIELD_IS_NULL");
            var res = await _userRepository.GetUser(target);
            if (res is null) throw new UserNotFoundException();

            var privacyDict = new Dictionary<string, string?>();
            var listField = fields.Split(',').ToList();
            foreach (var field in listField)
            {
                var value = res.GetPropertyValue(field);
                if (value is not null)
                {
                    privacyDict.Add(field, value?.ToString());
                }
            }

            var targetId = res.Id.ToString();
            var privacyLevels = await _userPrivacyRepository.GetPrivacyLevelsAsync(targetId, listField);

            foreach (var field in listField)
            {
                if (!privacyDict.ContainsKey(field))
                {
                    privacyDict[field] = null;
                    continue;
                }
                privacyDict[field] = privacyLevels[field] switch
                {
                    PrivacyLevel.Public => privacyDict[field],
                    PrivacyLevel.Private => null,
                    _ => null
                };
            }

            return Result<GetUserProfileDto>.Success(new GetUserProfileDto()
            {
                Infos = privacyDict,
                IsOwner = false
            });
        }

        /// <summary>
        /// Get user info by fields
        /// </summary>
        /// <param name="targetId"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public async Task<Result<GetUserProfileDto>> GetUserInfoPublicAsync(Guid targetId, string fields)
            => await GetUserInfoPublicAsync(targetId.ToString(), fields);


        /// <summary>
        /// Update a user info
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="request"></param>
        /// <returns> </returns>
        public async Task<Result<UpdateUserDto>> UpdateUserAsync(string userId, UpdateUserDto updateUserDto)
            => await UpdateUserAsync(Guid.Parse(userId), updateUserDto);


        /// <summary>
        /// Update a user info
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="updateUserDto"></param>
        /// <returns></returns>
        public async Task<Result<UpdateUserDto>> UpdateUserAsync(Guid userId, UpdateUserDto updateUserDto)
        {
            var existingUser = await _userRepository.GetUser(userId.ToString());
            if (existingUser is null) throw new UserNotFoundException();

            _mapper.Map(updateUserDto, existingUser);
            await _userRepository.UpdateUserAsync(existingUser);
            return Result<UpdateUserDto>.Success(updateUserDto);
        }

        public async Task<Result<ChangeUrlNameDto>> UpdateUrlNameAsync(Guid userId, ChangeUrlNameDto changeUrlNameDto)
        {
            ValidationHelper.EnsureValidUrlName(changeUrlNameDto.UrlName);

            var user = await _userRepository.GetUser(userId.ToString());
            if (user == null)
                throw new UserNotFoundException();
            var existUser = await _userRepository.GetUser(changeUrlNameDto.UrlName);

            if (existUser != null && existUser.Id != user.Id)
                throw new AppException("URLNAME_EXIST");

            user.UrlName = changeUrlNameDto.UrlName;
            await _userRepository.UpdateUserAsync(user);
            return Result<ChangeUrlNameDto>.Success(changeUrlNameDto);
        }

        public async Task<Result<ChangeUrlNameDto>> UpdateUrlNameAsync(string userId, ChangeUrlNameDto changeUrlNameDto)
            => await UpdateUrlNameAsync(userId.ToGuid(), changeUrlNameDto);

        public async Task<Result<ChangeNameDto>> UpdateNameAsync(string userId, ChangeNameDto changeNameDto)
        {
            var user = await _userRepository.GetUser(userId);
            if (user == null) throw new UserNotFoundException();

            user.FirstName = changeNameDto.FirstName;
            user.LastName = changeNameDto.LastName;
            user.FullName = $"{changeNameDto.FirstName} {changeNameDto.LastName}";
            await _userRepository.UpdateUserAsync(user);
            return Result<ChangeNameDto>.Success(changeNameDto);
        }
    }
}

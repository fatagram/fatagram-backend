using AutoMapper;
using Fatagram.Application.Dtos.User;
using Fatagram.Application.Services.UserServices.Interface;
using Fatagram.Application.Utils;
using Fatagram.Domain.Enums;
using Fatagram.Domain.Models;
using Fatagram.Domain.Utils;
using Fatagram.Infrastructure.Repositories.AccountRepository.Interface;
using Fatagram.Infrastructure.Repositories.UserPrivacyRepository.Interface;
using Fatagram.Infrastructure.Repositories.UserRepository.Interface;
using Fatagram.Shared.Extensions;
using Fatagram.Shared.Utils;

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

        private List<string> _canNotUpdateProperties = new List<string>()
        {
            nameof(User.Id),
        };

        public UserService(IAccountRepository accountRepository,
            IUserRepository userRepository, 
            IUserPrivacyRepository userPrivacyRepository, 
            IMapper mapper)
        {
            _accountRepository = accountRepository;
            _userRepository = userRepository;
            _userPrivacyRepository = userPrivacyRepository;
            _mapper = mapper;
        }


        /// <summary>
        /// Get user info by fields
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public async Task<Result<GetUserProfileDto>> GetUserInfoAuthenticatedAsync(string senderId, string target, string fields)
        {
            try
            {
                var res = await _userRepository.GetUser(target);
                if (res is null) return Result<GetUserProfileDto>.Failure("USER_NOT_FOUND");

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
            catch (Exception)
            {
                return Result<GetUserProfileDto>.Failure("GET_USER_INFO_FAILED");
            }
            
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
            try
            {
                var res = await _userRepository.GetUser(target);
                if (res is null) return Result<GetUserProfileDto>.Failure("USER_NOT_FOUND");

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
            catch (Exception)
            {
                return Result<GetUserProfileDto>.Failure("GET_USER_INFO_FAILED");
            }
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
        public async Task<Result<string>> UpdateUserAsync(string userId, UpdateUserDto updateUserDto)
            => await UpdateUserAsync(Guid.Parse(userId), updateUserDto);


        /// <summary>
        /// Update a user info
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="updateUserDto"></param>
        /// <returns></returns>
        public async Task<Result<string>> UpdateUserAsync(Guid userId, UpdateUserDto updateUserDto)
        {
            try
            {
                var existingUser = await _userRepository.GetUser(userId.ToString());
                if (existingUser is null) return Result<string>.Failure("USER_NOT_FOUND");
                // Update user
                var user = _mapper.Map<User>(updateUserDto);

                foreach (var prop in typeof(User).GetProperties())
                {
                    var value = prop.GetValue(user);
                    if (value is null || _canNotUpdateProperties.Contains(prop.Name)) continue;
                    prop.SetValue(existingUser, value);
                }
                await _userRepository.UpdateUserAsync(existingUser);

                return Result<string>.Success("UPDATE_USER_SUCCESS");
            }
            catch (Exception)
            {
                return Result<string>.Failure(ErrorCodes.UPDATE_USER_FAILED);
            }
        }
    }
}

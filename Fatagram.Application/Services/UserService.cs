using AutoMapper;
using Fatagram.Application.Dtos;
using Fatagram.Application.Services.Interfaces;
using Fatagram.Application.Utils;
using Fatagram.Domain.Models;
using Fatagram.Infrastructure.Repositories.Interfaces;
using System.IO.IsolatedStorage;

namespace Fatagram.Application.Services
{
    /// <summary>
    /// User service
    /// </summary>
    public class UserService : IUserService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserService(IAccountRepository accountRepository, IUserRepository userRepository, IMapper mapper)
        {
            _accountRepository = accountRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }


        /// <summary>
        /// Get user info by fields
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public async Task<Result<Dictionary<string,string>>> GetUserInfoByFieldsAsync(string userId, string fields)
        {
            string[] listField = fields.Split(',');
            var infos = await _userRepository.GetUserInfosWithFieldsAsync(userId, listField);
            var result = new Dictionary<string, string>();

            for(int i = 0; i < listField.Length; i++)
            {
                result.Add(listField[i], infos[i]);
            }

            return Result<Dictionary<string, string>>.Success(result);
        }


        /// <summary>
        /// Get user info by fields
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public async Task<Result<Dictionary<string, string>>> GetUserInfoByFieldsAsync(Guid userId, string fields) 
            => await GetUserInfoByFieldsAsync(userId.ToString(), fields);


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
            // Update user
            var user = _mapper.Map<User>(updateUserDto);
            user.Id = userId;
            var res = await _userRepository.UpdateUserAsync(user);
            if (!res) return Result<string>.Failure(ErrorCodes.UPDATE_USER_FAILED);

            return Result<string>.Success("UPDATE_USER_SUCCESS");
        }
    }
}

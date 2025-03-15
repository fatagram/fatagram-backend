using AutoMapper;
using Fatagram.Application.Dtos;
using Fatagram.Application.Services.Interfaces;
using Fatagram.Application.Utils;
using Fatagram.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.IdentityModel.Protocols;

namespace Fatagram.Application.Services
{
    public class OtherUserService : IOtherUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPrivarySettingRepository _privacySettingRepository;
        private readonly IMapper _mapper;

        public OtherUserService(IUserRepository userRepository, IPrivarySettingRepository privarySettingRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _privacySettingRepository = privarySettingRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Get a user by username
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<Result<UserDto>> GetOtherUserInfoByFieldsAsync(string userId, string fields)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user is null) return Result<UserDto>.Failure("OTHER_USER_NOT_FOUND");

            var userDto = _mapper.Map<UserDto>(user);
            var props = fields.Split(",").Select(p => p.Trim().ToLower()).ToArray();
            var userDtoProps = userDto.GetType().GetProperties();

            foreach(var userDtoProp in userDtoProps)
            {
                if (props.Contains(userDtoProp.Name.ToLower())) continue;
                userDtoProp.SetValue(userDto, null);
            }
            return Result<UserDto>.Success(userDto);
        }


        /// <summary>
        /// Get a user by id
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public async Task<Result<UserDto>> GetOtherUserInfoByFieldsAsync(Guid userId, string fields) => await GetOtherUserInfoByFieldsAsync(userId.ToString(), fields);
    }
}

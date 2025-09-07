using AutoMapper;
using Fatagram.Application.Dtos.User;
using Fatagram.Application.Dtos.User.Update;
using Fatagram.Application.Exceptions;
using Fatagram.Application.Services.UserServices.UserProfileServices.Interface;
using Fatagram.Application.Utils;
using Fatagram.Application.Validation;
using Fatagram.Infrastructure.Repositories.UserRepository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Fatagram.Application.Services.UserServices.UserProfileServices
{
    public class UserInfoService : IUserInfoService
    {
        private readonly IUserInformationRepository _userInfoRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserInfoService(
            IUserInformationRepository userInfoRepository, 
            IUserRepository userRepository,
            IMapper mapper)
        {
            _userInfoRepository = userInfoRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<Result<UserInfoOverview>> GetUserInfoAsync(Guid userId, Guid targetId)
        {
            var userInfo = await _userRepository.GetAsync(targetId.ToString());
            var userInfoV2 = await _userInfoRepository.GetUserInfoAsync(targetId);

            if (userInfo is null || userInfoV2 is null)
                throw new UserNotFoundException();

            var result = _mapper.Map<UserInfoOverview>(userInfo);
            result = _mapper.Map(userInfoV2, result);

            if (userId == targetId && userId != Guid.Empty)
                result.IsOwner = true;

            return Result<UserInfoOverview>.Success(result);
        }

        public async Task<Result<ChangeNicknameDto>> UpdateNicknameAsync(Guid userId, ChangeNicknameDto changeNicknameDto)
        {
            ValidationHelper.EnsureValidNickname(changeNicknameDto.Nickname);
            var user = await _userRepository.GetAsync(userId.ToString());
            if (user == null)
                throw new UserNotFoundException();

            var existUser = await _userRepository.GetAsync(changeNicknameDto.Nickname);
            if (existUser != null && existUser.Id != user.Id)
            {
                return Result<ChangeNicknameDto>.BadRequest("USERNAME_EXISTED", "Username existed");
            }
            if (string.IsNullOrEmpty(changeNicknameDto.Nickname))
            {
                user.Nickname = null;
            }
            else
            {
                user.Nickname = changeNicknameDto.Nickname;
            }
            await _userRepository.UpdateAsync(user);
            return Result<ChangeNicknameDto>.Success(changeNicknameDto);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Fatagram.Application.Dtos.User;
using Fatagram.Application.Dtos.User.Update;
using Fatagram.Application.Exceptions;
using Fatagram.Application.Services.UserServices.UserProfileServices.Interfaces;
using Fatagram.Application.Utils;
using Fatagram.Infrastructure.Repositories.UserRepository.Interface;
using Fatagram.Shared.Enums;

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
            IMapper mapper
        )
        {
            _userInfoRepository = userInfoRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<Result<UserInfoOverview>> GetUserInfoAsync(Guid userId, Guid targetId)
        {
            throw new NotImplementedException();
        }

        public async Task<Result<ChangeNicknameDto>> UpdateNicknameAsync(
            Guid userId,
            ChangeNicknameDto changeNicknameDto
        )
        {
            var user = await _userRepository.GetAsync(userId, u => u);
            if (user is null)
            {
                throw new UserNotFoundException();
            }
            user.Nickname = changeNicknameDto.Nickname;
            await _userRepository.UpdateAsync(user);
            return Result<ChangeNicknameDto>.Create(ResponseStatusCode.Success, changeNicknameDto);
        }
    }
}

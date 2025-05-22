using AutoMapper;
using Fatagram.Application.Dtos.User;
using Fatagram.Application.Services.UserPrivacyServices.Interface;
using Fatagram.Domain.Models;
using Fatagram.Infrastructure.Repositories.UserPrivacyRepository.Interface;
using Fatagram.Shared.Extensions;
using Fatagram.Shared.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fatagram.Application.Services.UserPrivacyServices
{
    public class UserPrivacyService : IUserPrivacyService
    {
        private readonly IUserPrivacyRepository _userPrivacyRepository;
        private readonly IMapper _mapper;
        public UserPrivacyService(IUserPrivacyRepository userPrivacyRepository, IMapper mapper)
        {
            _mapper = mapper;
            _userPrivacyRepository = userPrivacyRepository;
        }

        /// <summary>
        /// Get privacy levels for the specified fields of a user asynchronously.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="updateUserPrivacyDto"></param>
        /// <returns></returns>
        public async Task<Result<string>> UpdateUserPrivacyAsync(string userId, UpdateUserPrivacyDto updateUserPrivacyDto)
        {
            var userPrivacy = _mapper.Map<UserPrivacy>(updateUserPrivacyDto);
            userPrivacy.UserId = userId.ToGuid();
            await _userPrivacyRepository.UpdateAsync(userPrivacy);

            return Result<string>.Success();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fatagram.Application.Exceptions;
using Fatagram.Application.Exceptions.DetailExceptions;
using Fatagram.Application.Services.UserServices.UserConfigServices;
using Fatagram.Application.Utils;
using Fatagram.Application.Abstractions.Repositories;

namespace Fatagram.Application.Services.UserServices.UserConfigServices
{
    public class UserConfigService : IUserConfigService
    {
        private readonly IUserRepository _userRepository;

        public UserConfigService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<Result<string>> ChangeLanguage(Guid userId, string langCode)
        {
            var user =
                await _userRepository.GetAsync(userId, u => u) ?? throw new UserNotFoundException();
            user.LanguageCode = langCode;
            await _userRepository.UpdateAsync(user);

            return Result<string>.Create();
        }
    }
}

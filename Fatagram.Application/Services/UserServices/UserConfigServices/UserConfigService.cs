using Fatagram.Application.Exceptions;
using Fatagram.Application.Services.UserServices.UserConfigServices.Interfaces;
using Fatagram.Application.Utils;
using Fatagram.Infrastructure.Repositories.UserRepository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fatagram.Application.Services.UserServices.UserConfigServices
{
    public class UserConfigService : IUserConfigService
    {
        private readonly IUserRepository _userRepository;

        public UserConfigService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<Result<string>> ChangeLanguage(string userId, string langCode)
        {
            var user = await _userRepository.GetAsync(userId);
            if (user == null) throw new UserNotFoundException();

            user.LanguageCode = langCode;
            await _userRepository.UpdateAsync(user);

            return Result<string>.Success();
        }
    }
}

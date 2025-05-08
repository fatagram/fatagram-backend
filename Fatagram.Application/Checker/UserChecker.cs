using Fatagram.Application.Dtos.User;
using Fatagram.Infrastructure.Repositories.UserRepository.Interface;
using Microsoft.EntityFrameworkCore.Update.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fatagram.Application.Checker
{
    public class UserChecker
    {
        //private readonly IUserRepository _userRepository;
        //public UserChecker(IUserRepository userRepository)
        //{
        //    _userRepository = userRepository;
        //}

        //public async Task<List<string>> UpdateCheck(Guid userId, UpdateUserDto updateUserDto)
        //{
        //    if (updateUserDto.Username != null && !await CheckUsernameAsync(userId, updateUserDto.Username))
        //    {

        //    }
        //    return true;
        //}

        //private async Task<bool> CheckUsernameAsync(Guid userId, string username)
        //{
        //    var user = await _userRepository.GetUserByUrlNameAsync(username);
        //    if (user != null && user.Id != userId) return false;
        //    return true;
        //}
    }
}

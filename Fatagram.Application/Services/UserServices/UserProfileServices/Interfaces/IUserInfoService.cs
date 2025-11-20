using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Fatagram.Application.Dtos.User;
using Fatagram.Application.Dtos.User.Update;
using Fatagram.Application.Utils;

namespace Fatagram.Application.Services.UserServices.UserProfileServices.Interfaces
{
    public interface IUserInfoService
    {
        Task<Result<UserInfoOverview>> GetUserInfoAsync(Guid userId, Guid targetId);

        Task<Result<ChangeNicknameDto>> UpdateNicknameAsync(
            Guid userId,
            ChangeNicknameDto changeNicknameDto
        );
    }
}

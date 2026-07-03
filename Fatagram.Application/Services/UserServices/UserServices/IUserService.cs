using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.Application.Dtos.Filter;
using Fatagram.Application.Dtos.User;
using Fatagram.Application.Utils;

namespace Fatagram.Application.Services.UserServices.UserServices
{
    public interface IUserService
    {
        Task<Result<CursorResult<UserDto, DateTime>>> GetAllAsync(CursorFilter<DateTime> filter);
        Task<Result<CursorResult<SearchUserDto, DateTime>>> SearchAsync(
            Guid currentUserId,
            CursorFilter<DateTime> filter
        );
    }
}

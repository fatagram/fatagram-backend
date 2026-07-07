using Fatagram.Application.Dtos.Filter;
using Fatagram.Application.Dtos.User;
using Fatagram.Application.Utils;

namespace Fatagram.Application.Services.UserServices.UserCoreServices
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

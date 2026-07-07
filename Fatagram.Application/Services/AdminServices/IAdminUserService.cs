using Fatagram.Application.Dtos.Admin;
using Fatagram.Application.Utils;

namespace Fatagram.Application.Services.AdminServices
{
    public interface IAdminUserService
    {
        Task<Result<List<UserAdminDto>>> SearchUsersAsync(string query, CancellationToken ct = default);
        Task<Result<List<RoleDto>>> GetUserRolesAsync(Guid userId, CancellationToken ct = default);
        Task<Result> AssignRoleAsync(Guid userId, AssignRoleDto dto, CancellationToken ct = default);
        Task<Result> RemoveRoleAsync(Guid userId, Guid roleId, Guid? resourceId, CancellationToken ct = default);
    }
}

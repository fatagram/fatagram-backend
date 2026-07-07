using Fatagram.Application.Dtos.Admin;
using Fatagram.Application.Utils;

namespace Fatagram.Application.Services.AdminServices
{
    public interface IAdminRoleService
    {
        Task<Result<List<RoleDto>>> GetAllRolesAsync(CancellationToken ct = default);
        Task<Result<List<PermissionDto>>> GetAllPermissionsAsync(CancellationToken ct = default);
        Task<Result<RoleDto>> CreateRoleAsync(CreateRoleDto dto, CancellationToken ct = default);
        Task<Result> DeleteRoleAsync(Guid roleId, CancellationToken ct = default);
        Task<Result> AssignPermissionAsync(Guid roleId, Guid permissionId, CancellationToken ct = default);
        Task<Result> RemovePermissionAsync(Guid roleId, Guid permissionId, CancellationToken ct = default);
    }
}

using Fatagram.API.Utils;
using Fatagram.Application.Dtos.Admin;
using Fatagram.Application.Services.AdminServices;

namespace Fatagram.API.Controllers.V1.Admin
{
    [Route("api/v{version:apiVersion}/admin/roles")]
    public class AdminRoleController(IAdminRoleService service) : AdminBaseController
    {
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct)
            => (await service.GetAllRolesAsync(ct)).ToActionResult();

        [HttpGet("permissions")]
        public async Task<IActionResult> GetAllPermissions(CancellationToken ct)
            => (await service.GetAllPermissionsAsync(ct)).ToActionResult();

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateRoleDto dto, CancellationToken ct)
            => (await service.CreateRoleAsync(dto, ct)).ToActionResult();

        [HttpDelete("{roleId:guid}")]
        public async Task<IActionResult> Delete(Guid roleId, CancellationToken ct)
            => (await service.DeleteRoleAsync(roleId, ct)).ToActionResult();

        [HttpPost("{roleId:guid}/permissions/{permissionId:guid}")]
        public async Task<IActionResult> AssignPermission(Guid roleId, Guid permissionId, CancellationToken ct)
            => (await service.AssignPermissionAsync(roleId, permissionId, ct)).ToActionResult();

        [HttpDelete("{roleId:guid}/permissions/{permissionId:guid}")]
        public async Task<IActionResult> RemovePermission(Guid roleId, Guid permissionId, CancellationToken ct)
            => (await service.RemovePermissionAsync(roleId, permissionId, ct)).ToActionResult();
    }
}

using Fatagram.API.Utils;
using Fatagram.Application.Dtos.Admin;
using Fatagram.Application.Services.AdminServices;

namespace Fatagram.API.Controllers.V1.Admin
{
    [Route("api/v{version:apiVersion}/admin/users")]
    public class AdminUserController(IAdminUserService service) : AdminBaseController
    {
        [HttpGet]
        public async Task<IActionResult> Search([FromQuery] string q, CancellationToken ct) =>
            (await service.SearchUsersAsync(q ?? string.Empty, ct)).ToActionResult();

        [HttpGet("{userId:guid}/roles")]
        public async Task<IActionResult> GetUserRoles(Guid userId, CancellationToken ct) =>
            (await service.GetUserRolesAsync(userId, ct)).ToActionResult();

        [HttpPost("{userId:guid}/roles")]
        public async Task<IActionResult> AssignRole(
            Guid userId,
            [FromBody] AssignRoleDto dto,
            CancellationToken ct
        ) => (await service.AssignRoleAsync(userId, dto, ct)).ToActionResult();

        [HttpDelete("{userId:guid}/roles/{roleId:guid}")]
        public async Task<IActionResult> RemoveRole(
            Guid userId,
            Guid roleId,
            [FromQuery] Guid? resourceId,
            CancellationToken ct
        ) => (await service.RemoveRoleAsync(userId, roleId, resourceId, ct)).ToActionResult();
    }
}

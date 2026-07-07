using Fatagram.API.Utils;
using Fatagram.Application.Dtos.Admin;
using Fatagram.Application.Services.AdminServices;

namespace Fatagram.API.Controllers.V1.Admin
{
    [Route("api/v{version:apiVersion}/admin/route-permissions")]
    public class AdminRoutePermissionController(IAdminRoutePermissionService service)
        : AdminBaseController
    {
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct)
            => (await service.GetAllAsync(ct)).ToActionResult();

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateRoutePermissionDto dto, CancellationToken ct)
            => (await service.CreateAsync(dto, ct)).ToActionResult();

        [HttpPatch("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRoutePermissionDto dto, CancellationToken ct)
            => (await service.UpdateAsync(id, dto, ct)).ToActionResult();

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
            => (await service.DeleteAsync(id, ct)).ToActionResult();
    }
}

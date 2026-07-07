using Fatagram.Application.Dtos.Admin;
using Fatagram.Application.Utils;

namespace Fatagram.Application.Services.AdminServices
{
    public interface IAdminRoutePermissionService
    {
        Task<Result<List<RoutePermissionDto>>> GetAllAsync(CancellationToken ct = default);
        Task<Result<RoutePermissionDto>> CreateAsync(CreateRoutePermissionDto dto, CancellationToken ct = default);
        Task<Result<RoutePermissionDto>> UpdateAsync(Guid id, UpdateRoutePermissionDto dto, CancellationToken ct = default);
        Task<Result> DeleteAsync(Guid id, CancellationToken ct = default);
    }
}

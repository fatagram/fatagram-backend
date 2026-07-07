using Fatagram.Domain.Models;

namespace Fatagram.Application.Abstractions.Repositories
{
    public interface IRoutePermissionRepository : IBaseRepository<RoutePermission>
    {
        Task<IEnumerable<RoutePermission>> GetActivePermissionsAsync(
            CancellationToken ct = default
        );
    }
}

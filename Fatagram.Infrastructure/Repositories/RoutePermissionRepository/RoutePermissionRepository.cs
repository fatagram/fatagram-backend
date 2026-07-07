using Fatagram.Application.Abstractions.Repositories;
using Fatagram.Domain.Models;
using Fatagram.Infrastructure.Data;
using Fatagram.Infrastructure.Repositories.BaseRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Fatagram.Infrastructure.Repositories.RoutePermissionRepository
{
    public class RoutePermissionRepository(
        AppDbContext dbContext,
        ILogger<RoutePermissionRepository> logger
    ) : BaseRepository<RoutePermission>(dbContext, logger), IRoutePermissionRepository
    {
        public async Task<IEnumerable<RoutePermission>> GetActivePermissionsAsync(
            CancellationToken ct = default
        )
        {
            return await _dbContext
                .RoutePermissions.Where(rp => rp.IsActive)
                .OrderBy(rp => rp.Order)
                .ToListAsync(ct);
        }
    }
}

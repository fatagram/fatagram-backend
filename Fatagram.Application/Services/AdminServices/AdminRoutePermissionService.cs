using Fatagram.Application.Abstractions.Cache;
using Fatagram.Application.Abstractions.Repositories;
using Fatagram.Application.Dtos.Admin;
using Fatagram.Application.Exceptions.MiddleLevelExceptions;
using Fatagram.Application.Utils;
using Fatagram.Domain.Models;
using Fatagram.Shared.Common;
using Fatagram.Shared.Enums;

namespace Fatagram.Application.Services.AdminServices
{
    public class AdminRoutePermissionService(
        IRoutePermissionRepository routePermissionRepository,
        ICacheService cacheService
    ) : IAdminRoutePermissionService
    {
        private const string RouteCacheKey = "route_permissions:active";

        public async Task<Result<List<RoutePermissionDto>>> GetAllAsync(CancellationToken ct = default)
        {
            var list = await routePermissionRepository.GetAllAsync<RoutePermission>();
            return Result<List<RoutePermissionDto>>.Create(
                data: list.Select(RoutePermissionDto.From).OrderBy(r => r.Order).ToList());
        }

        public async Task<Result<RoutePermissionDto>> CreateAsync(CreateRoutePermissionDto dto, CancellationToken ct = default)
        {
            var entity = new RoutePermission
            {
                HttpMethod = dto.HttpMethod,
                RoutePattern = dto.RoutePattern,
                PermissionName = dto.PermissionName,
                ResourceParam = dto.ResourceParam,
                IsActive = dto.IsActive,
                Order = dto.Order,
            };
            var created = await routePermissionRepository.AddAsync(entity);
            await cacheService.RemoveAsync(RouteCacheKey);
            return Result<RoutePermissionDto>.Create(ResponseStatusCode.Created, RoutePermissionDto.From(created));
        }

        public async Task<Result<RoutePermissionDto>> UpdateAsync(Guid id, UpdateRoutePermissionDto dto, CancellationToken ct = default)
        {
            var entity = await routePermissionRepository.GetByUniqueAsync<RoutePermission>(
                e => e.Id == id, selector: null);
            if (entity is null)
                throw new NotFoundException(new Error("ROUTE_PERMISSION_NOT_FOUND", "Route permission not found"));

            if (entity.RoutePattern == "api/v1/admin/*" && dto.IsActive == false)
            {
                throw new BadRequestException(new Error("CANNOT_DISABLE_ADMIN_ROUTE", "Không thể vô hiệu hóa route bảo vệ Admin 'api/v1/admin/*'"));
            }

            if (dto.HttpMethod is not null) entity.HttpMethod = dto.HttpMethod;
            if (dto.RoutePattern is not null) entity.RoutePattern = dto.RoutePattern;
            if (dto.PermissionName is not null) entity.PermissionName = dto.PermissionName;
            if (dto.ResourceParam is not null) entity.ResourceParam = dto.ResourceParam;
            if (dto.IsActive is not null) entity.IsActive = dto.IsActive.Value;
            if (dto.Order is not null) entity.Order = dto.Order.Value;

            var updated = await routePermissionRepository.UpdateAsync(entity);
            await cacheService.RemoveAsync(RouteCacheKey);
            return Result<RoutePermissionDto>.Create(data: RoutePermissionDto.From(updated));
        }

        public async Task<Result> DeleteAsync(Guid id, CancellationToken ct = default)
        {
            var entity = await routePermissionRepository.GetByUniqueAsync<RoutePermission>(
                e => e.Id == id, selector: null);
            if (entity is not null && entity.RoutePattern == "api/v1/admin/*")
            {
                throw new BadRequestException(new Error("CANNOT_DELETE_ADMIN_ROUTE", "Không thể xóa route bảo vệ Admin 'api/v1/admin/*'"));
            }

            await routePermissionRepository.DeleteAsync(id);
            await cacheService.RemoveAsync(RouteCacheKey);
            return Result.Create();
        }
    }
}

using Fatagram.Domain.Models;

namespace Fatagram.Application.Dtos.Admin
{
    public record RoutePermissionDto(
        Guid Id,
        string HttpMethod,
        string RoutePattern,
        string PermissionName,
        string? ResourceParam,
        bool IsActive,
        int Order,
        DateTime CreatedAt
    )
    {
        public static RoutePermissionDto From(RoutePermission e) => new(
            e.Id, e.HttpMethod, e.RoutePattern, e.PermissionName,
            e.ResourceParam, e.IsActive, e.Order, e.CreatedAt);
    }

    public record CreateRoutePermissionDto(
        string HttpMethod,
        string RoutePattern,
        string PermissionName,
        string? ResourceParam,
        bool IsActive,
        int Order
    );

    public record UpdateRoutePermissionDto(
        string? HttpMethod,
        string? RoutePattern,
        string? PermissionName,
        string? ResourceParam,
        bool? IsActive,
        int? Order
    );
}

using Fatagram.Domain.Models;

namespace Fatagram.Application.Dtos.Admin
{
    public record PermissionDto(Guid Id, string Name, string Description)
    {
        public static PermissionDto From(Permission p) => new(p.Id, p.Name, p.Description);
    }

    public record RoleDto(
        Guid Id,
        string Name,
        string Description,
        bool IsSystem,
        List<PermissionDto> Permissions
    )
    {
        public static RoleDto From(Role r) => new(
            r.Id, r.Name, r.Description, r.IsSystem,
            r.Permissions.Select(PermissionDto.From).ToList());
    }

    public record CreateRoleDto(string Name, string Description);
}

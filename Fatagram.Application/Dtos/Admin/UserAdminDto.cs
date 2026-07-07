namespace Fatagram.Application.Dtos.Admin
{
    public record UserAdminDto(
        Guid Id,
        string? UrlName,
        string? FullName,
        string? Username,
        List<UserRoleDto> Roles
    );

    public record UserRoleDto(Guid RoleId, string RoleName, Guid? ResourceId);

    public record AssignRoleDto(Guid RoleId, Guid? ResourceId);
}

namespace Fatagram.Domain.Models
{
    public class Role : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsSystem { get; set; } = false;
        public ICollection<Permission> Permissions { get; set; } = new List<Permission>();
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}

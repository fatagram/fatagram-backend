namespace Fatagram.Domain.Models
{
    public class UserRole : BaseEntity
    {
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }

        /// <summary>Null = global scope. Non-null = scoped to a specific resource (e.g. a conversation).</summary>
        public Guid? ResourceId { get; set; }

        public User User { get; set; } = null!;
        public Role Role { get; set; } = null!;
    }
}

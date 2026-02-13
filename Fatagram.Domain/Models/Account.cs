using Fatagram.Domain;
using Fatagram.Domain.Enums;

namespace Fatagram.Domain.Models
{
    /// <summary>
    /// Represents an account in the system.
    /// </summary>
    public class Account : BaseEntity
    {
        /// <summary>
        /// Gets or sets the username for the account.
        /// </summary>
        public string? Username { get; set; }

        /// <summary>
        /// Gets or sets the password hash for the account.
        /// </summary>
        public string? PasswordHash { get; set; }

        /// <summary>
        /// Gets or sets the authentication provider for the account.
        /// </summary>
        // public AuthProvider AuthProvider { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the account is active.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the associated user.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets the associated user.
        /// </summary>
        public User User { get; set; } = null!;
    }
}

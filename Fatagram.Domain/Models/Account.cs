using Fatagram.Domain;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fatagram.Domain.Models
{
    /// <summary>
    /// Represents an account in the system.
    /// </summary>
    [Table("accounts")]
    [Index(nameof(Username), IsUnique = true)]
    public class Account
    {
        /// <summary>
        /// Gets or sets the unique identifier for the account.
        /// </summary>
        [Column("id", TypeName = "uuid")]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the username for the account.
        /// </summary>
        [Column("username", TypeName = "varchar(50)")]
        [Required]
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the password hash for the account.
        /// </summary>
        [Column("password_hash", TypeName = "varchar(100)")]
        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether the account is active.
        /// </summary>
        [Column("is_active", TypeName = "boolean")]
        [Required]
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Gets or sets the date and time when the account was created.
        /// </summary>
        [Column("created_at", TypeName = "timestamptz")]
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets the date and time when the account was last updated.
        /// </summary>
        [Column("updated_at", TypeName = "timestamptz")]
        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets the unique identifier of the associated user.
        /// </summary>
        [Column("user_id", TypeName = "uuid")]
        [Required]
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets the version of the account entity for concurrency control.
        /// </summary>
        [Timestamp]
        [Column("xmin")]
        public uint Version { get; set; } = 0;

        /// <summary>
        /// Gets or sets the associated user.
        /// </summary>
        public User? User { get; set; }

        /// <summary>
        /// Gets or sets the refresh token for the account.
        /// </summary>
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}

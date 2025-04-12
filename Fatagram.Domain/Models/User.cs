using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;

namespace Fatagram.Domain.Models
{
    /// <summary>
    /// Represents a user in the system.
    /// </summary>
    [Table("users")]
    [Index(nameof(Email), IsUnique = true)]
    [Index(nameof(Username), IsUnique = true)]
    public class User
    {
        /// <summary>
        /// Gets or sets the unique identifier for the user.
        /// </summary>
        [Column("id", TypeName = "uniqueidentifier")]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the username of the user.
        /// </summary>
        [Column("username", TypeName = "nvarchar(50)")]
        public string? Username { get; set; }

        /// <summary>
        /// Gets or sets the last name of the user.
        /// </summary>
        [Column("last_name", TypeName = "nvarchar(50)")]
        [Required]
        public string LastName { get; set; } = null!;

        /// <summary>
        /// Gets or sets the first name of the user.
        /// </summary>
        [Column("first_name", TypeName = "nvarchar(50)")]
        [Required]
        public string FirstName { get; set; } = null!;

        /// <summary>
        /// Gets or sets the full name of the user.
        /// </summary>
        [Column("full_name", TypeName = "nvarchar(100)")]
        [Required]
        public string FullName { get; set; } = null!;

        /// <summary>
        /// Gets or sets the email address of the user.
        /// </summary>
        [Column("email", TypeName = "nvarchar(100)")]
        [Required]
        public string Email { get; set; } = null!;

        /// <summary>
        /// Gets or sets the phone number of the user.
        /// </summary>
        [Column("phone", TypeName = "varchar(10)")]
        public string? Phone { get; set; }

        /// <summary>
        /// Gets or sets the bio of the user.
        /// </summary>
        [Column("bio", TypeName = "nvarchar(max)")]
        public string? Bio { get; set; }

        /// <summary>
        /// Gets or sets the avatar URL of the user.
        /// </summary>
        [Column("avatar", TypeName = "nvarchar(max)")]
        public string? Avatar { get; set; }

        /// <summary>
        /// Gets or sets the background image URL of the user.
        /// </summary>
        [Column("background", TypeName = "nvarchar(max)")]
        public string? Background { get; set; }

        /// <summary>
        /// Gets or sets the birth date of the user.
        /// </summary>
        [Column("birth_day", TypeName = "date")]
        public DateTime? BirthDay { get; set; }

        /// <summary>
        /// Gets or sets the account associated with the user.
        /// </summary>
        
        public ICollection<Account> Accounts { get; set; } = new List<Account>();


        /// <summary>
        /// Gets or sets the posts created by the user.
        /// </summary>
        public ICollection<UserPrivacy> Privacies { get; set; } = new List<UserPrivacy>();
    }
}

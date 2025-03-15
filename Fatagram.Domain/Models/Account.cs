using Fatagram.Domain;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fatagram.Domain.Models
{
    public class Account
    {
        [Column("id")]
        [Key]
        public Guid Id { get; set; }

        [Column("username")]
        [Required]
        public string Username { get; set; } = string.Empty;

        [Column("password_hash")]
        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Column("is_active")]
        [Required]
        public bool IsActive { get; set; } = true;

        [Column("created_at")]
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        // 1 - 1 reltionship with User

        [Column("user_id")]
        [Required]
        public Guid UserId { get; set; }

        public User? User { get; set; }
    }
}

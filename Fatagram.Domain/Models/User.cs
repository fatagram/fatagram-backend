using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fatagram.Domain.Models
{
    public class User
    {
        /// <summary>
        /// Id
        /// </summary>
        [Column("id")]
        [Key]
        public Guid Id { get; set; }


        [Column("last_name")]
        [Required]
        public string LastName { get; set; } = string.Empty;

        [Column("first_name")]
        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Column("full_name")]
        [Required]
        public string FullName { get; set; } = string.Empty;

        [Column("email")]
        [Required]
        public string Email { get; set; } = string.Empty;

        [Column("phone")]
        public string? Phone { get; set; }

        [Column("bio")]
        public string? Bio { get; set; } 

        [Column("avatar")]
        public string? Avatar { get; set; } 

        [Column("birth_day")]
        public DateTime? BirthDay { get; set; } 
        
        // 1-1 relationship with Account
        public Account? Account { get; set; }
    }
}

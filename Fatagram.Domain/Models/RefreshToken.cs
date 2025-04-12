using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fatagram.Domain.Models
{
    /// <summary>
    /// Refresh token model
    /// </summary>
    [Table("refresh_tokens")]
    public class RefreshToken
    {
        /// <summary>
        /// Token
        /// </summary>
        [Column("token", TypeName= "uniqueidentifier")]
        [Key]
        [Required]
        public Guid Token { get; set; }

        /// <summary>
        /// Account id
        /// </summary>
        [Column("account_id", TypeName = "uniqueidentifier")]
        [Required]
        public Guid AccountId { get; set; }


        /// <summary>
        /// Expiry date
        /// </summary>
        [Column("expiry_date", TypeName = "date")]
        [Required]
        public DateTime ExpiryDate { get; set; }


        /// <summary>
        /// Created at
        /// </summary>
        [Column("created_at", TypeName = "date")]
        public DateTime CreatedAt { get; set; }


        /// <summary>
        /// Account
        /// </summary>
        public Account Account { get; set; } = null!;

    }
}

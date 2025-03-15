using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fatagram.Domain.Models
{
    /// <summary>
    /// Refresh token model
    /// </summary>
    public class RefreshToken
    {

        /// <summary>
        /// Token
        /// </summary>
        [Column("token")]
        [Key]
        public string Token { get; set; } = string.Empty;

        /// <summary>
        /// Account id
        /// </summary>
        [Column("account_id")]
        [Required]
        public Guid AccountId { get; set; }


        /// <summary>
        /// Expiry date
        /// </summary>
        [Column("expiry_date")]
        [Required]
        public DateTime ExpiryDate { get; set; }


        /// <summary>
        /// Created at
        /// </summary>
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

    }
}

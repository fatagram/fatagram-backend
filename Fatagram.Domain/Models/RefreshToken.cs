namespace Fatagram.Domain.Models
{
    /// <summary>
    /// Refresh token model
    /// </summary>
    public class RefreshToken : BaseEntity
    {
        /// <summary>
        /// Token
        /// </summary>
        public string Token { get; set; } = null!;

        /// <summary>
        /// Account id
        /// </summary>
        public Guid AccountId { get; set; }

        /// <summary>
        /// Expiry date
        /// </summary>
        public DateTime ExpiresAt { get; set; }

        /// <summary>
        /// Account
        /// </summary>
        public Account Account { get; set; } = null!;
    }
}

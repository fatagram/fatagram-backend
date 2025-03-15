namespace Fatagram.Domain.Utils
{
    /// <summary>
    /// Response for authentication
    /// </summary>
    public class RefreshTokenGenerateResult
    {
        /// <summary>
        /// Refresh token
        /// </summary>
        public string Token { get; set; } = string.Empty;

        /// <summary>
        /// Expiry date
        /// </summary>
        public DateTime ExpiryDate { get; set; } = DateTime.MinValue;

        /// <summary>
        /// Create date
        /// </summary>
        public DateTime CreateDate { get; set; } = DateTime.UtcNow;
    }
}

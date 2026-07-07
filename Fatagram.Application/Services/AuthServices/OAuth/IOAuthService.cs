namespace Fatagram.Application.Services.AuthServices.OAuth
{
    /// <summary>
    /// OAuth user information DTO
    /// </summary>
    public class OAuthUserInfo
    {
        public required string Email { get; set; }
        public string? Name { get; set; }
        public string? GivenName { get; set; }
        public string? FamilyName { get; set; }
        public string? Picture { get; set; }
    }

    /// <summary>
    /// Interface for OAuth service providers
    /// </summary>
    public interface IOAuthService
    {
        /// <summary>
        /// Get user information from OAuth provider using authorization code
        /// </summary>
        /// <param name="code">OAuth authorization code</param>
        /// <returns>OAuth user information</returns>
        Task<OAuthUserInfo> GetUserInfoAsync(string code, string? redirectUriOverride = null);
    }
}

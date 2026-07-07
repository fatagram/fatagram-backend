namespace Fatagram.Application.Dtos.Auth
{
    /// <summary>
    /// Generic OAuth callback DTO
    /// </summary>
    public record OAuthCallbackDto
    {
        /// <summary>
        /// OAuth authorization code returned from OAuth provider
        /// </summary>
        public required string Code { get; set; }

        /// <summary>
        /// Override redirect URI — must match what was used in the auth request.
        /// Falls back to GoogleOAuth:RedirectUri from config when omitted.
        /// </summary>
        public string? RedirectUri { get; set; }
    }
}

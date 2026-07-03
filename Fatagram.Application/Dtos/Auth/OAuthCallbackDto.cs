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
    }
}

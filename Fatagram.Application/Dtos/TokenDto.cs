using System.ComponentModel.DataAnnotations;

namespace Fatagram.Application.Dtos
{
    /// <summary>
    /// Response for authentication
    /// </summary>
    public class TokenDto
    {
        /// <summary>
        /// Refresh token
        /// </summary>
        [Required]
        public string RefreshToken { get; set; } = string.Empty;

        /// <summary>
        /// Access token
        /// </summary>
        /// 
        [Required]
        public string AccessToken { get; set; } = string.Empty;
    }
}

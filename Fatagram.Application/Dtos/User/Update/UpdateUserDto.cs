using System.ComponentModel.DataAnnotations;
using Fatagram.Application.Utils;

namespace Fatagram.Application.Dtos.User.Update
{
    /// <summary>
    /// Data transfer object for updating user
    /// </summary>
    public record UpdateUserDto
    {
        public string? Username { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Bio { get; set; }
        public string? Description { get; set; }
        public string? Avatar { get; set; }
        public string? Background { get; set; }
        public Dictionary<string, object?>? BackgroundMetadata { get; set; }
        public string? Phone { get; set; }
    }
}

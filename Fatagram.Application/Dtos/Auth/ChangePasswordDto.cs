using System.ComponentModel.DataAnnotations;
using Fatagram.Application.Utils;

namespace Fatagram.Application.Dtos.Auth
{
    /// <summary>
    /// Data transfer object for changing password
    /// </summary>
    public class ChangePasswordDto
    {
        public string OldPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}

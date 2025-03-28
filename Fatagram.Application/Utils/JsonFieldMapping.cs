using Fatagram.Domain.Models;
using System.Data.Common;
using System.Runtime.CompilerServices;

namespace Fatagram.Domain.Utils
{
    /// <summary>
    /// Fields for the user model
    /// </summary>
    public static class JsonFieldMapping
    {
        public static readonly Dictionary<string, string> Fields = new Dictionary<string, string>
        {
            { "id", nameof(User.Id) },
            { "firstName", nameof(User.FirstName) },
            { "lastName", nameof(User.LastName) },
            { "fullName", nameof(User.FullName) },
            { "email", nameof(User.Email) },
            { "phone", nameof(User.Phone) },
            { "bio", nameof(User.Bio) },
            { "avatar", nameof(User.Avatar) },
            { "background", nameof(User.Background) },
            { "birthday", nameof(User.BirthDay) }
        };

    }
}

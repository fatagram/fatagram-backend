using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fatagram.Shared.Extensions
{
    /// <summary>
    /// Provides extension methods for the <see cref="Guid"/> structure.
    /// </summary>
    public static class GuidExtensions
    {
        /// <summary>
        /// Determines whether the specified <see cref="Guid"/> is empty.
        /// </summary>
        /// <param name="guid">The <see cref="Guid"/> to check.</param>
        /// <returns><c>true</c> if the <see cref="Guid"/> is empty; otherwise, <c>false</c>.</returns>
        public static bool IsEmpty(this Guid guid)
        {
            return guid == Guid.Empty;
        }

        /// <summary>
        /// Converts the specified string representation of a GUID to its <see cref="Guid"/> equivalent.
        /// </summary>
        /// <param name="value">The string representation of the GUID.</param>
        /// <returns>A <see cref="Guid"/> equivalent to the value contained in <paramref name="value"/>, or <see cref="Guid.Empty"/> if the conversion failed.</returns>
        public static Guid ToGuid(this string? value)
        {
            if (value is null)
            {
                return Guid.Empty;
            }
            if (Guid.TryParse(value, out Guid result))
            {
                return result;
            }
            return Guid.Empty;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fatagram.Shared.Extensions
{
    /// <summary>
    /// Provides extension methods for enumerations.
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// Converts the string representation of the name or numeric value of one or more enumerated constants to an equivalent enumerated object.
        /// </summary>
        /// <typeparam name="T">An enumeration type.</typeparam>
        /// <param name="value">A string containing the name or value to convert.</param>
        /// <returns>An object of type T whose value is represented by value. If the conversion fails, the default value of T is returned.</returns>
        public static T ToEnum<T>(this string? value) where T : struct, Enum
        {
            if (value is null)
            {
                return default;
            }
            if (Enum.TryParse<T>(value, out T result))
            {
                return result;
            }
            return default;
        }
    }
}

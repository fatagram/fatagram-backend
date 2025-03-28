using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Fatagram.Shared.Extensions
{
    /// <summary>
    /// Provides extension methods for objects.
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// Gets the value of a property by name from an object.
        /// </summary>
        /// <param name="obj">The object from which to get the property value.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The value of the property if found; otherwise, null.</returns>
        public static object? GetPropertyValue(this object obj, string propertyName)
        {
            var prop = obj.GetType().GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            return prop?.GetValue(obj);
        }
    }
}

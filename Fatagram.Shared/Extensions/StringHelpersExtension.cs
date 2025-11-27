using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fatagram.Shared.Extensions
{
    public static class StringHelpersExtension
    {
        /// <summary>
        /// Normalize empty string properties in dto to null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dto"></param>
        public static T NormalizeEmptyStringToNull<T>(this T dto)
        {
            foreach (var prop in dto!.GetType().GetProperties())
            {
                if (prop.PropertyType == typeof(string))
                {
                    var value = (string?)prop.GetValue(dto);
                    if (value != null && value == "")
                    {
                        prop.SetValue(dto, null);
                    }
                }
            }
            return dto;
        }

        public static string ToLowerFirstLetter(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return str;
            return $"{char.ToLower(str[0])}{str[1..]}";
        }
    }
}

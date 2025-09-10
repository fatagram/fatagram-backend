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
            foreach(var prop in dto!.GetType().GetProperties())
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
    }
}

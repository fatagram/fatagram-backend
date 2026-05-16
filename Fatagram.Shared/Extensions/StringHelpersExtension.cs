using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

        public static string RemoveVietnameseTone(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return string.Empty;

            str = str.ToLower();

            char[] chars = str.ToCharArray();

            for (int i = 0; i < chars.Length; i++)
            {
                chars[i] = chars[i] switch
                {
                    'á'
                    or 'à'
                    or 'ả'
                    or 'ã'
                    or 'ạ'
                    or 'ă'
                    or 'ắ'
                    or 'ằ'
                    or 'ẳ'
                    or 'ẵ'
                    or 'ặ'
                    or 'â'
                    or 'ấ'
                    or 'ầ'
                    or 'ẩ'
                    or 'ẫ'
                    or 'ậ' => 'a',
                    'é' or 'è' or 'ẻ' or 'ẽ' or 'ẹ' or 'ê' or 'ế' or 'ề' or 'ể' or 'ễ' or 'ệ' =>
                        'e',
                    'í' or 'ì' or 'ỉ' or 'ĩ' or 'ị' => 'i',
                    'ó'
                    or 'ò'
                    or 'ỏ'
                    or 'õ'
                    or 'ọ'
                    or 'ô'
                    or 'ố'
                    or 'ồ'
                    or 'ổ'
                    or 'ỗ'
                    or 'ộ'
                    or 'ơ'
                    or 'ớ'
                    or 'ờ'
                    or 'ở'
                    or 'ỡ'
                    or 'ợ' => 'o',
                    'ú' or 'ù' or 'ủ' or 'ũ' or 'ụ' or 'ư' or 'ứ' or 'ừ' or 'ử' or 'ữ' or 'ự' =>
                        'u',
                    'ý' or 'ỳ' or 'ỷ' or 'ỹ' or 'ỵ' => 'y',
                    'đ' => 'd',
                    _ => chars[i],
                };
            }

            return new string(chars);
        }
    }
}

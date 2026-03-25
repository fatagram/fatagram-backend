using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Fatagram.Application.Utils
{
    public static class ConversationUtils
    {
        public static string GenerateUniqueConversationKey(Guid u1, Guid u2)
        {
            var first = u1.CompareTo(u2) <= 0 ? u1 : u2;
            var second = u1.CompareTo(u2) <= 0 ? u2 : u1;

            string rawKey = $"{first:N}_{second:N}";

            using (var md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(rawKey);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                return Convert.ToHexString(hashBytes).ToLower();
            }
        }
    }
}

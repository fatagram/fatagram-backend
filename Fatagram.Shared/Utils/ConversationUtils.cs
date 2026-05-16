using System;
using System.Linq;

namespace Fatagram.Shared.Utils
{
    public static class ConversationUtils
    {
        public static string GenerateUniqueConversationKey(Guid user1Id, Guid user2Id)
        {
            var ids = new[] { user1Id, user2Id }.OrderBy(id => id).ToList();
            return $"{ids[0]}:{ids[1]}";
        }
    }
}

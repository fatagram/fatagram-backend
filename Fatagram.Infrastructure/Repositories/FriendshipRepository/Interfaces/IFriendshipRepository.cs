using Fatagram.Domain.Models;
using Fatagram.Infrastructure.Projections;
using Npgsql.EntityFrameworkCore.PostgreSQL.Query.ExpressionTranslators.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fatagram.Infrastructure.Repositories.FriendshipRepository.Interfaces
{
    public interface IFriendshipRepository
    {
        Task AddAsync(Friendship friendship);
        Task DeleteAsync(Friendship friendship);
        Task<Friendship?> GetAsync(Guid user1Id, Guid user2Id);
        Task<int> CountAsync(Guid userId);
        Task<(IEnumerable<FriendProjection> friends, int total)> GetFriendsOfUserAsync(Guid userId, Guid targetId, string? keyword, int page = 1, int pageSize = 10);
    }
}

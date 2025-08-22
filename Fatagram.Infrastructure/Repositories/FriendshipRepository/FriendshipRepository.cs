using Fatagram.Domain.Models;
using Fatagram.Infrastructure.Data;
using Fatagram.Infrastructure.Projections;
using Fatagram.Infrastructure.Repositories.FriendshipRepository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Fatagram.Infrastructure.Repositories.FriendshipRepository
{
    public class FriendshipRepository : IFriendshipRepository
    {
        private readonly AppDbContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        public FriendshipRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(Friendship friendship)
        {
            await _dbContext.Friendships.AddAsync(friendship);
            await _dbContext.SaveChangesAsync();
        }

        public Task DeleteAsync(Friendship friendship)
        {
            _dbContext.Friendships.Remove(friendship);
            return _dbContext.SaveChangesAsync();
        }

        public Task<Friendship?> GetAsync(Guid user1Id, Guid user2Id)
        {
            return _dbContext.Friendships.FirstOrDefaultAsync(f => 
                (f.User1Id == user1Id && f.User2Id == user2Id)
                || (f.User1Id == user2Id && f.User2Id == user1Id));
        }

        public async Task<int> CountAsync(Guid userId)
        {
            return await _dbContext.Friendships
                .Where(f => f.User1Id == userId || f.User2Id == userId)
                .CountAsync();
        }

        public async Task<(IEnumerable<FriendProjection> friends, int total)> GetFriendsOfUserAsync(Guid userId, Guid targetId, string? keyword, int page = 1, int pageSize = 10)
        {
            var friendIdsQuery = _dbContext.Friendships
                .Where(f => f.User1Id == targetId || f.User2Id == targetId)
                .Select(f => f.User1Id == targetId ? f.User2Id : f.User1Id);

            var query = _dbContext.Users
                .Where(u => friendIdsQuery.Any(f => f == u.Id) && 
                (string.IsNullOrEmpty(keyword) || u.FullName.ToLower().Contains(keyword.ToLower())));

            var total = await query.CountAsync();

            // paging and attach isfriend tag

            var result = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new FriendProjection
                {
                    User = u,
                    IsFriend = _dbContext.Friendships
                        .Any(f => (f.User1Id == userId && f.User2Id == u.Id) ||
                        (f.User1Id == u.Id && f.User2Id == userId))
                })
                .ToListAsync();

            return (result, total);
        }
    }
}

using Fatagram.Domain.Models;
using Fatagram.Infrastructure.Data;
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

        public async Task<(IEnumerable<User> users, int total)> GetFriendsOfUserAsync(Guid userId, string? keyword, int page = 1, int pageSize = 10)
        {
            // Lấy friendships có liên quan đến user
            var friendships = await _dbContext.Friendships
                .Where(f => f.User1Id == userId || f.User2Id == userId)
                .Include(f => f.User1)
                .Include(f => f.User2)
                .ToListAsync();

            // Chuyển sang danh sách bạn bè (User còn lại)
            var friends = friendships
                .Select(f => f.User1Id == userId ? f.User2 : f.User1)
                .AsQueryable();

            // Nếu có keyword, lọc theo FullName (lọc sau khi đã lấy từ DB -> client-side filter)
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var lowerKeyword = keyword.Trim().ToLower();
                friends = friends.Where(u => u.FullName.ToLower().Contains(lowerKeyword));
            }

            var total = friends.Count();

            var users = friends
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return (users, total);
        }
    }
}

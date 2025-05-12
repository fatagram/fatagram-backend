using Fatagram.Domain.Models;
using Fatagram.Infrastructure.Data;
using Fatagram.Infrastructure.Repositories.FriendRequestRepository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fatagram.Infrastructure.Repositories.FriendRequestRepository
{
    public class FriendRequestRepository : IFriendRequestRepository
    {
        private readonly AppDbContext _dbContext;

        public FriendRequestRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(FriendRequest friendRequest)
        {
            await _dbContext.FriendRequests.AddAsync(friendRequest);
            await _dbContext.SaveChangesAsync();
        }

        public Task DeleteAsync(FriendRequest friendRequest)
        {
            _dbContext.FriendRequests.Remove(friendRequest);
            return _dbContext.SaveChangesAsync();
        }

        public async Task<FriendRequest?> GetAsync(Guid senderId, Guid receiverId)
        {
            return await _dbContext.FriendRequests
                .FirstOrDefaultAsync(fr => fr.SenderId == senderId && fr.ReceiverId == receiverId);
        }

        public async Task<(List<FriendRequest> requests, int total)> GetFriendRequestsAsync(Guid userId, int page, int pageSize)
        {
            var total = await _dbContext.FriendRequests
                .CountAsync(fr => fr.ReceiverId == userId);

            var friendRequests = await _dbContext.FriendRequests
                .Where(fr => fr.ReceiverId == userId)
                .Include(fr => fr.Sender)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (friendRequests, total);
        }
    }
}

using System;
using Fatagram.Domain.Models;
using Fatagram.Infrastructure.Data;
using Fatagram.Infrastructure.Repositories.BaseRepository;
using Fatagram.Infrastructure.Repositories.FriendRequestRepository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Fatagram.Infrastructure.Repositories.FriendRequestRepository
{
    public class FriendRequestRepository : BaseRepository<FriendRequest>, IFriendRequestRepository
    {
        public FriendRequestRepository(AppDbContext dbContext)
            : base(dbContext) { }

        public async Task<bool> RequestExistsAsync(Guid senderId, Guid receiverId)
        {
            var exists = await _dbSet.AnyAsync(fr =>
                fr.SenderId == senderId && fr.ReceiverId == receiverId
                || fr.SenderId == receiverId && fr.ReceiverId == senderId
            );
            return exists;
        }
    }
}

using System;
using Fatagram.Domain.Models;
using Fatagram.Infrastructure.Data;
using Fatagram.Infrastructure.Repositories.BaseRepository;
using Fatagram.Application.Abstractions.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Fatagram.Infrastructure.Repositories.FriendshipRepository
{
    public class FriendshipRepository : BaseRepository<Friendship>, IFriendshipRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        public FriendshipRepository(AppDbContext dbContext)
            : base(dbContext) { }

        public async Task<bool> AreFriendsAsync(Guid user1Id, Guid user2Id)
        {
            var friends = await _dbSet.AnyAsync(fr =>
                (fr.User1Id == user1Id && fr.User2Id == user2Id)
                || (fr.User1Id == user2Id && fr.User2Id == user1Id)
            );
            return friends;
        }
    }
}

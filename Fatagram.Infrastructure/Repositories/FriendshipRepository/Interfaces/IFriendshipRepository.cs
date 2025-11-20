using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fatagram.Domain.Models;
using Fatagram.Infrastructure.Projections;
using Fatagram.Infrastructure.Repositories.BaseRepository.Interfaces;
using Npgsql.EntityFrameworkCore.PostgreSQL.Query.ExpressionTranslators.Internal;

namespace Fatagram.Infrastructure.Repositories.FriendshipRepository.Interfaces
{
    public interface IFriendshipRepository : IBaseRepository<Friendship>
    {
        public Task<bool> AreFriendsAsync(Guid user1Id, Guid user2Id);
    }
}

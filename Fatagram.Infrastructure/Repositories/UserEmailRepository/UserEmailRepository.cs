using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.Domain.Models;
using Fatagram.Infrastructure.Data;
using Fatagram.Infrastructure.Repositories.BaseRepository;
using Fatagram.Infrastructure.Repositories.UserEmailRepository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Fatagram.Infrastructure.Repositories.UserEmailRepository
{
    public class UserEmailRepository : BaseRepository<UserEmail>, IUserEmailRepository
    {
        public UserEmailRepository(
            AppDbContext dbContext,
            ILogger<BaseRepository<UserEmail>>? logger = null
        )
            : base(dbContext, logger) { }

        public async Task<bool> IsEmailInUseAsync(string email)
        {
            return await _dbSet.Where(ue => ue.Email.Address == email).AnyAsync();
        }
    }
}

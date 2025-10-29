using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fatagram.Domain.Models.UserInformations;
using Fatagram.Infrastructure.Data;
using Fatagram.Infrastructure.Projections;
using Fatagram.Infrastructure.Repositories.UserRepository.Interface;
using Microsoft.EntityFrameworkCore;

namespace Fatagram.Infrastructure.Repositories.UserRepository
{
    public class UserInformationRepository : IUserInformationRepository
    {
        private AppDbContext _dbContext;

        public UserInformationRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Hobby>> GetHobbiesAsync(Guid userId, string langCode = "en")
        {
            var res = await _dbContext
                .Users.Include(u => u.Hobbies)
                .FirstOrDefaultAsync(u => u.Id == userId);

            return res?.Hobbies ?? Enumerable.Empty<Hobby>();
        }

        public async Task<IEnumerable<Skill>> GetSkillsAsync(Guid userId, string langCode = "en")
        {
            var res = await _dbContext
                .Users.Include(u => u.Skills)
                .FirstOrDefaultAsync(u => u.Id == userId);

            return res?.Skills ?? Enumerable.Empty<Skill>();
        }

        public async Task<UserInfoV2> GetUserInfoAsync(Guid userId)
        {
            return await _dbContext
                    .Users.Where(u => u.Id == userId)
                    .Select(u => new UserInfoV2()
                    {
                        Nickname = u.Nickname,
                        Description = u.Description,
                    })
                    .FirstOrDefaultAsync() ?? new UserInfoV2();
        }
    }
}

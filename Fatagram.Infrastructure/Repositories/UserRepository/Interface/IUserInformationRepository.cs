using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fatagram.Domain.Models.UserInformations;
using Fatagram.Infrastructure.Projections;

namespace Fatagram.Infrastructure.Repositories.UserRepository.Interface
{
    public interface IUserInformationRepository
    {
        Task<IEnumerable<Hobby>> GetHobbiesAsync(Guid userId, string langCode = "en");
        Task<IEnumerable<Skill>> GetSkillsAsync(Guid userId, string langCode = "en");
        Task<UserInfoV2> GetUserInfoAsync(Guid userId);
    }
}

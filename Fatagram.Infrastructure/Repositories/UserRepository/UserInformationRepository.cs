using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fatagram.Domain.Models;
using Fatagram.Infrastructure.Data;
using Fatagram.Infrastructure.Projections;
using Fatagram.Infrastructure.Repositories.BaseRepository;
using Fatagram.Infrastructure.Repositories.UserRepository.Interface;
using Microsoft.EntityFrameworkCore;

namespace Fatagram.Infrastructure.Repositories.UserRepository
{
    public class UserInformationRepository : BaseRepository<User>, IUserInformationRepository
    {
        public UserInformationRepository(AppDbContext dbContext)
            : base(dbContext) { }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.Domain.Models;
using Fatagram.Infrastructure.Data;
using Fatagram.Infrastructure.Repositories.BaseRepository;
using Fatagram.Application.Abstractions.Repositories;

namespace Fatagram.Infrastructure.Repositories.EmailRepository
{
    public class EmailRepository : BaseRepository<Email>, IEmailRepository
    {
        public EmailRepository(AppDbContext dbContext)
            : base(dbContext) { }
    }
}

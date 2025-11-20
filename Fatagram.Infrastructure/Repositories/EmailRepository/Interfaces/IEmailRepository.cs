using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.Domain.Models;
using Fatagram.Infrastructure.Repositories.BaseRepository.Interfaces;

namespace Fatagram.Infrastructure.Repositories.EmailRepository.Interfaces
{
    public interface IEmailRepository : IBaseRepository<Email> { }
}

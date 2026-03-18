using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.Domain.Models;
using Fatagram.Infrastructure.Data;
using Fatagram.Infrastructure.Repositories.BaseRepository;
using Fatagram.Infrastructure.Repositories.MessageRepository.Interfaces;
using Microsoft.Extensions.Logging;

namespace Fatagram.Infrastructure.Repositories.MessageRepository
{
    public class MessageRepository : BaseRepository<Message>, IMessageRepository
    {
        public MessageRepository(
            AppDbContext dbContext,
            ILogger<BaseRepository<Message>>? logger = null
        )
            : base(dbContext, logger) { }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.Domain.Models;
using Fatagram.Infrastructure.Data;
using Fatagram.Infrastructure.Repositories.BaseRepository;
using Fatagram.Infrastructure.Repositories.NotificationRepository.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Fatagram.Infrastructure.Repositories.NotificationRepository
{
    public class UserNotificationRepository
        : BaseRepository<UserNotification>,
            IUserNotificationRepository
    {
        public UserNotificationRepository(
            AppDbContext dbContext,
            ILogger<BaseRepository<UserNotification>>? logger = null
        )
            : base(dbContext, logger) { }
    }
}

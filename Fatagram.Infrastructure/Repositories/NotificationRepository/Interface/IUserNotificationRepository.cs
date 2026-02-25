using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.Domain.Models;
using Fatagram.Infrastructure.Repositories.BaseRepository.Interfaces;

namespace Fatagram.Infrastructure.Repositories.NotificationRepository.Interface
{
    public interface IUserNotificationRepository : IBaseRepository<UserNotification> { }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fatagram.Domain.Enums.NotificationServices;
using Fatagram.Domain.Models;
using Fatagram.Infrastructure.Data;
using Fatagram.Infrastructure.Repositories.BaseRepository;
using Fatagram.Infrastructure.Repositories.NotificationRepository.Interface;
using Microsoft.EntityFrameworkCore;

namespace Fatagram.Infrastructure.Repositories.NotificationRepository
{
    public class NotificationContentRepository
        : BaseRepository<NotificationContent>,
            INotificationContentRepository
    {
        public NotificationContentRepository(AppDbContext context)
            : base(context) { }
    }
}

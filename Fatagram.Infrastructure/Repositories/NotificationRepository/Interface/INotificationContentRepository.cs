using Fatagram.Domain.Enums.NotificationServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fatagram.Infrastructure.Repositories.NotificationRepository.Interface
{
    public interface INotificationContentRepository
    {
        Task<string> GetContentAsync(NotificationType type, string langCode);
    }
}
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fatagram.Domain.Enums.NotificationServices;
using Fatagram.Infrastructure.Data;
using Fatagram.Infrastructure.Repositories.NotificationRepository.Interface;
using Microsoft.EntityFrameworkCore;

namespace Fatagram.Infrastructure.Repositories.NotificationRepository
{
    public class NotificationContentRepository : INotificationContentRepository
    {
        private readonly AppDbContext _context;

        public NotificationContentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<string> GetContentAsync(NotificationType type, string langCode)
        {
            return await _context
                    .NotificationContents.Where(nc =>
                        nc.Type == type && nc.LanguageCode == langCode
                    )
                    .Select(nc => nc.Content)
                    .FirstOrDefaultAsync() ?? string.Empty;
        }
    }
}

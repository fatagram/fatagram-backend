using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fatagram.Application.Abstractions.Repositories;
using Fatagram.Domain.Models;
using Fatagram.Infrastructure.Data;
using Fatagram.Infrastructure.Repositories.BaseRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Fatagram.Infrastructure.Repositories.ChatThemeRepository
{
    public class ChatThemeRepository(
        AppDbContext dbContext,
        ILogger<ChatThemeRepository>? logger = null
    ) : BaseRepository<ChatTheme>(dbContext, logger), IChatThemeRepository
    {
        public async Task<List<ChatTheme>> GetActiveThemesAsync(CancellationToken ct = default)
        {
            var now = DateTime.UtcNow;
            return await _dbSet
                .AsNoTracking()
                .Where(t =>
                    t.IsActive
                    && (t.StartDate == null || t.StartDate <= now)
                    && (t.EndDate == null || t.EndDate >= now)
                )
                .OrderBy(t => t.SortOrder)
                .ThenBy(t => t.CreatedAt)
                .ToListAsync(ct);
        }

        public async Task<List<ChatTheme>> GetAllForAdminAsync(CancellationToken ct = default)
        {
            return await _dbSet
                .AsNoTracking()
                .OrderBy(t => t.SortOrder)
                .ThenBy(t => t.CreatedAt)
                .ToListAsync(ct);
        }

        public async Task<ChatTheme?> GetByKeyAsync(string key, CancellationToken ct = default)
        {
            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Key == key, ct);
        }

        public async Task<bool> ExistsKeyAsync(string key, Guid? excludeId = null, CancellationToken ct = default)
        {
            var query = _dbSet.AsNoTracking().Where(t => t.Key == key);
            if (excludeId.HasValue)
            {
                query = query.Where(t => t.Id != excludeId.Value);
            }
            return await query.AnyAsync(ct);
        }
    }
}

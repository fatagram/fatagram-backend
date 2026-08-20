using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Fatagram.Domain.Models;

namespace Fatagram.Application.Abstractions.Repositories
{
    public interface IChatThemeRepository : IBaseRepository<ChatTheme>
    {
        Task<List<ChatTheme>> GetActiveThemesAsync(CancellationToken ct = default);
        Task<List<ChatTheme>> GetAllForAdminAsync(CancellationToken ct = default);
        Task<ChatTheme?> GetByKeyAsync(string key, CancellationToken ct = default);
        Task<bool> ExistsKeyAsync(string key, Guid? excludeId = null, CancellationToken ct = default);
    }
}

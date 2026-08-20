using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Fatagram.Application.Dtos.ChatTheme;
using Fatagram.Application.Utils;
using Fatagram.Shared.Common;

namespace Fatagram.Application.Services.ChatThemeServices
{
    public interface IChatThemeService
    {
        Task<Result<List<ChatThemeDto>>> GetActiveThemesAsync(CancellationToken ct = default);
        Task<Result<ChatThemeDto?>> GetThemeByKeyAsync(string key, CancellationToken ct = default);
        Task<Result<List<ChatThemeDto>>> GetAllThemesForAdminAsync(CancellationToken ct = default);
        Task<Result<ChatThemeDto>> GetThemeByIdForAdminAsync(Guid id, CancellationToken ct = default);
        Task<Result<ChatThemeDto>> CreateThemeAsync(CreateChatThemeDto dto, CancellationToken ct = default);
        Task<Result<ChatThemeDto>> UpdateThemeAsync(Guid id, UpdateChatThemeDto dto, CancellationToken ct = default);
        Task<Result<ChatThemeDto>> ToggleThemeStatusAsync(Guid id, CancellationToken ct = default);
        Task<Result> DeleteThemeAsync(Guid id, CancellationToken ct = default);
    }
}

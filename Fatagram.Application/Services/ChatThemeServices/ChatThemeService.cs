using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Fatagram.Application.Abstractions.Repositories;
using Fatagram.Application.Dtos.ChatTheme;
using Fatagram.Application.Exceptions.MiddleLevelExceptions;
using Fatagram.Application.Utils;
using Fatagram.Domain.Models;
using Fatagram.Shared.Common;
using Fatagram.Shared.Enums;

namespace Fatagram.Application.Services.ChatThemeServices
{
    public class ChatThemeService(IChatThemeRepository themeRepository) : IChatThemeService
    {
        public async Task<Result<List<ChatThemeDto>>> GetActiveThemesAsync(CancellationToken ct = default)
        {
            var themes = await themeRepository.GetActiveThemesAsync(ct);
            var dtos = themes.Select(MapToDto).ToList();
            return Result<List<ChatThemeDto>>.Create(data: dtos);
        }

        public async Task<Result<ChatThemeDto?>> GetThemeByKeyAsync(string key, CancellationToken ct = default)
        {
            var theme = await themeRepository.GetByKeyAsync(key, ct);
            return Result<ChatThemeDto?>.Create(data: theme != null ? MapToDto(theme) : null);
        }

        public async Task<Result<List<ChatThemeDto>>> GetAllThemesForAdminAsync(CancellationToken ct = default)
        {
            var themes = await themeRepository.GetAllForAdminAsync(ct);
            var dtos = themes.Select(MapToDto).ToList();
            return Result<List<ChatThemeDto>>.Create(data: dtos);
        }

        public async Task<Result<ChatThemeDto>> GetThemeByIdForAdminAsync(Guid id, CancellationToken ct = default)
        {
            var theme = await themeRepository.GetByUniqueAsync<ChatTheme>(t => t.Id == id);
            if (theme == null)
            {
                throw new NotFoundException(new Error("THEME_NOT_FOUND", "Chat theme not found."));
            }
            return Result<ChatThemeDto>.Create(data: MapToDto(theme));
        }

        public async Task<Result<ChatThemeDto>> CreateThemeAsync(CreateChatThemeDto dto, CancellationToken ct = default)
        {
            var trimmedKey = dto.Key.Trim().ToLowerInvariant();
            if (await themeRepository.ExistsKeyAsync(trimmedKey, null, ct))
            {
                throw new BadRequestException(new Error("DUPLICATE_KEY", "Theme key already exists."));
            }

            var lightJson = dto.Light != null ? JsonSerializer.Serialize(dto.Light) : null;
            var darkJson = dto.Dark != null ? JsonSerializer.Serialize(dto.Dark) : null;

            var entity = new ChatTheme
            {
                Key = trimmedKey,
                Label = dto.Label.Trim(),
                Category = string.IsNullOrWhiteSpace(dto.Category) ? "General" : dto.Category.Trim(),
                IsDefault = dto.IsDefault,
                IsActive = dto.IsActive,
                IsEvent = dto.IsEvent,
                BgImage = dto.BgImage,
                LightColorsJson = lightJson,
                DarkColorsJson = darkJson,
                StartDate = dto.StartDate.HasValue ? DateTime.SpecifyKind(dto.StartDate.Value, DateTimeKind.Utc) : null,
                EndDate = dto.EndDate.HasValue ? DateTime.SpecifyKind(dto.EndDate.Value, DateTimeKind.Utc) : null,
                SortOrder = dto.SortOrder,
                CreatedAt = DateTime.UtcNow,
            };

            var created = await themeRepository.AddAsync(entity);
            return Result<ChatThemeDto>.Create(ResponseStatusCode.Created, MapToDto(created));
        }

        public async Task<Result<ChatThemeDto>> UpdateThemeAsync(Guid id, UpdateChatThemeDto dto, CancellationToken ct = default)
        {
            var theme = await themeRepository.GetByUniqueAsync<ChatTheme>(t => t.Id == id);
            if (theme == null)
            {
                throw new NotFoundException(new Error("THEME_NOT_FOUND", "Chat theme not found."));
            }

            var lightJson = dto.Light != null ? JsonSerializer.Serialize(dto.Light) : null;
            var darkJson = dto.Dark != null ? JsonSerializer.Serialize(dto.Dark) : null;

            theme.Label = dto.Label.Trim();
            theme.Category = string.IsNullOrWhiteSpace(dto.Category) ? "General" : dto.Category.Trim();
            theme.IsDefault = dto.IsDefault;
            theme.IsActive = dto.IsActive;
            theme.IsEvent = dto.IsEvent;
            theme.BgImage = dto.BgImage;
            theme.LightColorsJson = lightJson;
            theme.DarkColorsJson = darkJson;
            theme.StartDate = dto.StartDate.HasValue ? DateTime.SpecifyKind(dto.StartDate.Value, DateTimeKind.Utc) : null;
            theme.EndDate = dto.EndDate.HasValue ? DateTime.SpecifyKind(dto.EndDate.Value, DateTimeKind.Utc) : null;
            theme.SortOrder = dto.SortOrder;
            theme.UpdatedAt = DateTime.UtcNow;

            var updated = await themeRepository.UpdateAsync(theme);
            return Result<ChatThemeDto>.Create(data: MapToDto(updated));
        }

        public async Task<Result<ChatThemeDto>> ToggleThemeStatusAsync(Guid id, CancellationToken ct = default)
        {
            var theme = await themeRepository.GetByUniqueAsync<ChatTheme>(t => t.Id == id);
            if (theme == null)
            {
                throw new NotFoundException(new Error("THEME_NOT_FOUND", "Chat theme not found."));
            }

            theme.IsActive = !theme.IsActive;
            theme.UpdatedAt = DateTime.UtcNow;

            var updated = await themeRepository.UpdateAsync(theme);
            return Result<ChatThemeDto>.Create(data: MapToDto(updated));
        }

        public async Task<Result> DeleteThemeAsync(Guid id, CancellationToken ct = default)
        {
            var theme = await themeRepository.GetByUniqueAsync<ChatTheme>(t => t.Id == id);
            if (theme == null)
            {
                throw new NotFoundException(new Error("THEME_NOT_FOUND", "Chat theme not found."));
            }

            if (theme.IsDefault)
            {
                throw new BadRequestException(new Error("CANNOT_DELETE_DEFAULT", "Cannot delete the default theme."));
            }

            await themeRepository.DeleteAsync(id);
            return Result.Create();
        }

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
        };

        private static ChatThemeDto MapToDto(ChatTheme entity)
        {
            ThemeColorsDto? light = null;
            ThemeColorsDto? dark = null;

            if (!string.IsNullOrWhiteSpace(entity.LightColorsJson))
            {
                try
                {
                    light = JsonSerializer.Deserialize<ThemeColorsDto>(entity.LightColorsJson, _jsonOptions);
                }
                catch
                {
                    // Fallback to null if malformed
                }
            }

            if (!string.IsNullOrWhiteSpace(entity.DarkColorsJson))
            {
                try
                {
                    dark = JsonSerializer.Deserialize<ThemeColorsDto>(entity.DarkColorsJson, _jsonOptions);
                }
                catch
                {
                    // Fallback to null if malformed
                }
            }

            return new ChatThemeDto
            {
                Id = entity.Id,
                Key = entity.Key,
                Label = entity.Label,
                Category = entity.Category,
                IsDefault = entity.IsDefault,
                IsActive = entity.IsActive,
                IsEvent = entity.IsEvent,
                BgImage = entity.BgImage,
                Light = light,
                Dark = dark,
                StartDate = entity.StartDate,
                EndDate = entity.EndDate,
                SortOrder = entity.SortOrder,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
            };
        }
    }
}

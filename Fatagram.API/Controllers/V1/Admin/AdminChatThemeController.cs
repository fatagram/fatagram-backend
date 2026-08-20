using System;
using System.Threading;
using System.Threading.Tasks;
using Fatagram.API.Utils;
using Fatagram.Application.Dtos.ChatTheme;
using Fatagram.Application.Services.ChatThemeServices;
using Microsoft.AspNetCore.Mvc;

namespace Fatagram.API.Controllers.V1.Admin
{
    [Route("api/v{version:apiVersion}/admin/chat-themes")]
    public class AdminChatThemeController(IChatThemeService chatThemeService) : AdminBaseController
    {
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var result = await chatThemeService.GetAllThemesForAdminAsync(ct);
            return result.ToActionResult();
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        {
            var result = await chatThemeService.GetThemeByIdForAdminAsync(id, ct);
            return result.ToActionResult();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateChatThemeDto dto, CancellationToken ct)
        {
            var result = await chatThemeService.CreateThemeAsync(dto, ct);
            return result.ToActionResult();
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateChatThemeDto dto, CancellationToken ct)
        {
            var result = await chatThemeService.UpdateThemeAsync(id, dto, ct);
            return result.ToActionResult();
        }

        [HttpPatch("{id:guid}/toggle")]
        public async Task<IActionResult> Toggle(Guid id, CancellationToken ct)
        {
            var result = await chatThemeService.ToggleThemeStatusAsync(id, ct);
            return result.ToActionResult();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        {
            var result = await chatThemeService.DeleteThemeAsync(id, ct);
            return result.ToActionResult();
        }
    }
}

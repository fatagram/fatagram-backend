using System.Threading;
using System.Threading.Tasks;
using Fatagram.API.Utils;
using Fatagram.Application.Services.ChatThemeServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fatagram.API.Controllers.V1
{
    [Authorize]
    [Route("api/v{version:apiVersion}/chat-themes")]
    public class ChatThemeController(IChatThemeService chatThemeService) : BaseApiController
    {
        [HttpGet]
        public async Task<IActionResult> GetActiveThemes(CancellationToken ct)
        {
            var result = await chatThemeService.GetActiveThemesAsync(ct);
            return result.ToActionResult();
        }

        [HttpGet("{key}")]
        public async Task<IActionResult> GetByKey(string key, CancellationToken ct)
        {
            var result = await chatThemeService.GetThemeByKeyAsync(key, ct);
            return result.ToActionResult();
        }
    }
}

using System.Threading.Tasks;
using Fatagram.Application.Services.GifServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fatagram.API.Controllers.V1
{
    [Authorize]
    public class GifController(IGifService gifService) : BaseApiController
    {
        private readonly IGifService _gifService = gifService;

        /// <summary>
        /// Search for GIFs based on a query
        /// </summary>
        /// <param name="query">Search term</param>
        /// <param name="limit">Number of results to return</param>
        /// <param name="pos">Cursor position for next page</param>
        /// <returns></returns>
        [HttpGet("search")]
        public async Task<IActionResult> Search(
            [FromQuery] string query,
            [FromQuery] int limit = 20,
            [FromQuery] string? pos = null
        )
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest("Query parameter is required.");
            }

            var result = await _gifService.SearchGifsAsync(query, limit, pos);
            return Ok(result);
        }

        /// <summary>
        /// Get trending GIFs
        /// </summary>
        /// <param name="limit">Number of results to return</param>
        /// <param name="pos">Cursor position for next page</param>
        /// <returns></returns>
        [HttpGet("trending")]
        public async Task<IActionResult> Trending(
            [FromQuery] int limit = 20,
            [FromQuery] string? pos = null
        )
        {
            var result = await _gifService.GetTrendingGifsAsync(limit, pos);
            return Ok(result);
        }
    }
}

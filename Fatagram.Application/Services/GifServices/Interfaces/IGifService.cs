using System.Threading.Tasks;
using Fatagram.Application.Dtos.Gif;

namespace Fatagram.Application.Services.GifServices.Interfaces
{
    public interface IGifService
    {
        Task<GifResponseDto> SearchGifsAsync(string query, int limit = 20, string? pos = null);
        Task<GifResponseDto> GetTrendingGifsAsync(int limit = 20, string? pos = null);
    }
}

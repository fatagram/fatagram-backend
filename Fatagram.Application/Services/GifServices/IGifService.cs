using System.Threading.Tasks;
using Fatagram.Application.Dtos.Gif;

namespace Fatagram.Application.Services.GifServices
{
    public interface IGifService
    {
        Task<GifsDto> SearchGifsAsync(string query, int limit = 20, string? pos = null);
        Task<GifsDto> GetTrendingGifsAsync(int limit = 20, string? pos = null);
    }
}

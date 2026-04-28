using System.Collections.Generic;

namespace Fatagram.Application.Dtos.Gif
{
    public class GifResponseDto
    {
        public List<GifDto> Gifs { get; set; } = [];
        public string? Next { get; set; }
    }
}

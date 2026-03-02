using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.Application.Services.ImageService.Enum;
using Fatagram.Application.Utils;
using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Processing;

namespace Fatagram.Application.Services.ImageService
{
    public static class ImageProcessor
    {
        public static ImageProcessingSpec BuildSpec(ImageRequest request)
        {
            var (w, h) = ResolveSize(request);

            return new ImageProcessingSpec
            {
                Width = request.Width ?? w,
                Height = request.Height ?? h,
                ResizeMode = request.ResizeMode,
                Format = request.Format,
                Quality = (int)request.Quality,
            };
        }

        private static (int w, int h) ResolveSize(ImageRequest request)
        {
            return request.SizePreset switch
            {
                ImageSizePreset.Thumbnail => (150, 150),
                ImageSizePreset.Small => (320, 240),
                ImageSizePreset.Medium => (640, 480),
                ImageSizePreset.Large => (1024, 768),
                ImageSizePreset.Original => (request.Width ?? 0, request.Height ?? 0),
                _ => (150, 150),
            };
        }
    }
}

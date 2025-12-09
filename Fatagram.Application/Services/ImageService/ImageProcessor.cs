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
        public static async Task<Image> ProcessImageAsync(ImageRequest request)
        {
            var imageEncoder = CreateImageEncoder(request);
            var (w, h) = ResolveSize(request);

            // if (!Directory.Exists(folder))
            // {
            //     Directory.CreateDirectory(folder);
            // }

            // var path = Path.Combine(folder, $"{fileName}.{request.Format.ToString().ToLower()}");

            request.ImageStream.Position = 0;
            var image = await Image.LoadAsync(request.ImageStream);

            // Resize
            image.Mutate(x =>
                x.Resize(
                    new ResizeOptions
                    {
                        Mode = MapResizeMode(request),
                        Size = new Size(request.Width ?? w, request.Height ?? h),
                    }
                )
            );

            return image;
        }

        public static async Task<string> SaveImageOnCloud(ImageRequest request)
        {
            //TODO: upload logic
            return "cloud_image_path";
        }

        private static (int w, int h) ResolveSize(ImageRequest request)
        {
            return request.SizePreset switch
            {
                ImageSizePreset.Thumbnail => (150, 150),
                ImageSizePreset.Small => (320, 240),
                ImageSizePreset.Medium => (640, 480),
                ImageSizePreset.Large => (1024, 768),
                ImageSizePreset.Original => (request.Width ?? 150, request.Height ?? 150),
                _ => (150, 150),
            };
        }

        private static ResizeMode MapResizeMode(ImageRequest request)
        {
            return request.ResizeMode switch
            {
                ImageResizeMode.Crop => ResizeMode.Crop,
                ImageResizeMode.Pad => ResizeMode.Pad,
                ImageResizeMode.BoxPad => ResizeMode.BoxPad,
                ImageResizeMode.Max => ResizeMode.Max,
                ImageResizeMode.Min => ResizeMode.Min,
                _ => ResizeMode.Max,
            };
        }

        private static IImageEncoder CreateImageEncoder(ImageRequest request)
        {
            return request.Format switch
            {
                ImageFormat.Jpeg => new SixLabors.ImageSharp.Formats.Jpeg.JpegEncoder
                {
                    Quality = (int)request.Quality,
                },
                ImageFormat.Png => new SixLabors.ImageSharp.Formats.Png.PngEncoder(),
                ImageFormat.Gif => new SixLabors.ImageSharp.Formats.Gif.GifEncoder(),
                ImageFormat.Bmp => new SixLabors.ImageSharp.Formats.Bmp.BmpEncoder(),
                _ => new SixLabors.ImageSharp.Formats.Jpeg.JpegEncoder
                {
                    Quality = (int)request.Quality,
                },
            };
        }
    }
}

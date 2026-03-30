using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.Application.Services.ImageService.Enum;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace Fatagram.Application.Services.ImageService
{
    public sealed class ImageSharpAdapter
    {
        public static async Task<Image> ProcessAsync(Stream stream, ImageProcessingSpec spec)
        {
            stream.Position = 0;
            var image = await Image.LoadAsync(stream);

            // Only resize when both width and height are provided (> 0).
            if (spec.Width > 0 && spec.Height > 0)
            {
                image.Mutate(x =>
                    x.Resize(
                        new ResizeOptions
                        {
                            Mode = MapResizeMode(spec.ResizeMode),
                            Size = new Size(spec.Width, spec.Height),
                        }
                    )
                );
            }

            return image;
        }

        private static ResizeMode MapResizeMode(ImageResizeMode mode) =>
            mode switch
            {
                ImageResizeMode.Crop => ResizeMode.Crop,
                ImageResizeMode.Pad => ResizeMode.Pad,
                ImageResizeMode.BoxPad => ResizeMode.BoxPad,
                ImageResizeMode.Max => ResizeMode.Max,
                ImageResizeMode.Min => ResizeMode.Min,
                _ => ResizeMode.Max,
            };
    }
}

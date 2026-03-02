using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CloudinaryDotNet;
using Fatagram.Application.Services.ImageService.Enum;

namespace Fatagram.Application.Services.ImageService
{
    public sealed class CloudinaryAdapter
    {
        public static Transformation BuildTransformation(ImageProcessingSpec spec)
        {
            return new Transformation()
                .Width(spec.Width)
                .Height(spec.Height)
                .Crop(MapCrop(spec.ResizeMode))
                .Quality(spec.Quality)
                .FetchFormat(MapFormat(spec.Format));
        }

        private static string MapCrop(ImageResizeMode mode) =>
            mode switch
            {
                ImageResizeMode.Crop => "fill",
                ImageResizeMode.Pad => "pad",
                ImageResizeMode.BoxPad => "pad",
                ImageResizeMode.Max => "limit",
                ImageResizeMode.Min => "scale",
                _ => "limit",
            };

        private static string MapFormat(ImageFormat format)
        {
            return format switch
            {
                ImageFormat.Jpeg => "jpg",
                ImageFormat.Png => "png",
                ImageFormat.Gif => "gif",
                ImageFormat.Bmp => "bmp",
                _ => format.ToString().ToLower(),
            };
        }
    }
}

using System;
using Fatagram.Application.Abstractions.Storage;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.Application.Abstractions.Storage.Enums;
using Fatagram.Application.Utils;
using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Processing;

namespace Fatagram.Infrastructure.Storage
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
                ImageSizePreset.Original => GetOriginalSize(request.ImageStream),
                _ => (150, 150),
            };
        }

        private static (int w, int h) GetOriginalSize(Stream stream)
        {
            if (stream == null || stream.Length == 0)
                return (0, 0);

            long originalPosition = stream.Position;
            try
            {
                var imageInfo = Image.Identify(stream);
                if (imageInfo != null)
                {
                    return (imageInfo.Width, imageInfo.Height);
                }
                else
                {
                    return (0, 0);
                }
            }
            catch (Exception)
            {
                return (0, 0);
            }
            finally
            {
                stream.Position = originalPosition;
            }
        }
    }
}

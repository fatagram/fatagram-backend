using System;
using Fatagram.Application.Abstractions.Storage;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.Application.Abstractions.Storage.Enums;

namespace Fatagram.Application.Abstractions.Storage
{
    public class ImageRequest
    {
        public Stream ImageStream { get; set; } = null!;
        public ImageQuality Quality { get; set; } = ImageQuality.Low;
        public ImageSizePreset SizePreset { get; set; } = ImageSizePreset.Original;
        public ImageFormat Format { get; set; } = ImageFormat.Jpeg;
        public ImageResizeMode ResizeMode { get; set; } = ImageResizeMode.Max;
        public int? Width { get; set; }
        public int? Height { get; set; }
    }
}

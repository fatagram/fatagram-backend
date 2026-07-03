using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.Application.Abstractions.Storage.Enums;

namespace Fatagram.Infrastructure.Storage
{
    public sealed class ImageProcessingSpec
    {
        public int Width { get; init; }
        public int Height { get; init; }
        public ImageResizeMode ResizeMode { get; init; }
        public ImageFormat Format { get; init; }
        public int Quality { get; init; }
    }
}

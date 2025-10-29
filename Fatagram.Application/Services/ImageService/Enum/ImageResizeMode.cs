using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fatagram.Application.Services.ImageService.Enum
{
    public enum ImageResizeMode
    {
        Crop,
        Pad,
        BoxPad,
        Max,
        Min,
        Stretch,
    }
}

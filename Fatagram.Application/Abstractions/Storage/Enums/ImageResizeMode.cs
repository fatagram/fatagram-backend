using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fatagram.Application.Abstractions.Storage.Enums
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

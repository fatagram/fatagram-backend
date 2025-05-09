using Fatagram.Application.Services.ImageService.Enum;
using Fatagram.Shared.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Fatagram.Application.Services.ImageService.Interface
{
    public interface IImageService
    {
        Task<Result<string>> SaveImageAsync(
            ImageSize size, 
            Stream imageStream, 
            string fileExtension, 
            string folder,
            bool isAvatar = false);

    }
}

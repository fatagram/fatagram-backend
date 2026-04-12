using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Fatagram.Application.Services.ImageService.Enum;
using Fatagram.Application.Utils;

namespace Fatagram.Application.Services.ImageService.Interface
{
    public interface IImageService
    {
        Task<Result<string>> SaveImageAsync(ImageRequest request, string folder);
        Task<Result<object>> GetUploadSignatureAsync(string folder, string resourceType = "auto");
    }
}

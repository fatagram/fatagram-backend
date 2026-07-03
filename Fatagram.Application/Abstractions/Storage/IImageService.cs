using System;
using Fatagram.Application.Abstractions.Storage;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Fatagram.Application.Abstractions.Storage.Enums;
using Fatagram.Application.Utils;

namespace Fatagram.Application.Abstractions.Storage
{
    public interface IImageService
    {
        Task<Result<string>> SaveImageAsync(ImageRequest request, string folder);
        Task<Result<object>> GetUploadSignatureAsync(string folder, string resourceType = "auto");
    }
}

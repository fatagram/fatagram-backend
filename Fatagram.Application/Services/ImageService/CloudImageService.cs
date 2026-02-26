using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.Application.Services.ImageService.Interface;
using Fatagram.Application.Utils;

namespace Fatagram.Application.Services.ImageService
{
    public class CloudImageService : IImageService
    {
        public Task<Result<string>> SaveImageAsync(ImageRequest request, string folder)
        {
            throw new NotImplementedException();
        }
    }
}

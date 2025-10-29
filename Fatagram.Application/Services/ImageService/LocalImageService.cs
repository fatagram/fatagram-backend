using System;
using System.IO;
using System.Threading.Tasks;
using Fatagram.Application.Services.ImageService.Enum;
using Fatagram.Application.Services.ImageService.Interface;
using Fatagram.Application.Utils;
using Fatagram.Shared.Enums;
using Fatagram.Shared.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace Fatagram.Application.Services.ImageService
{
    public class LocalImageService : IImageService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _storagePath;

        public LocalImageService(IConfiguration config, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _storagePath = config["ImageLocalStorage:Path"] ?? "";
        }

        public async Task<Result<string>> SaveImageAsync(ImageRequest request)
        {
            request.Folder = Path.Combine(_storagePath, request.Folder);
            var res = await ImageProcessor.SaveImageOnLocal(request);
            var finalUrl = Path.Combine(
                _httpContextAccessor.HttpContext?.Request.Scheme ?? "http",
                _httpContextAccessor.HttpContext?.Request.Host.ToString() ?? "localhost",
                _httpContextAccessor.HttpContext?.Request.PathBase.ToString() ?? "",
                res.Replace("\\", "/").Replace(_storagePath.Replace("\\", "/"), "").TrimStart('/')
            );
            return Result<string>.Create(ResponseStatusCode.Success, finalUrl);
        }
    }
}

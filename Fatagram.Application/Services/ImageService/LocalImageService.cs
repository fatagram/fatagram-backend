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
    public class LocalImageService(IConfiguration config, IHttpContextAccessor httpContextAccessor)
        : IImageService
    {
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly string _storagePath = config["LocalStorage:Path"] ?? "";

        public async Task<Result<string>> SaveImageAsync(ImageRequest request, string folder)
        {
            var fullFolderPath = Path.Combine(_storagePath, folder);
            var fileName = GenerateFileName();
            var filePath = Path.Combine(
                fullFolderPath,
                $"{fileName}.{request.Format.ToString().ToLower()}"
            );

            if (!Directory.Exists(fullFolderPath))
            {
                Directory.CreateDirectory(fullFolderPath);
            }

            var image = await ImageProcessor.ProcessImageAsync(request);

            await image.SaveAsync(filePath);

            var scheme = _httpContextAccessor.HttpContext?.Request.Scheme ?? "http";
            var host = _httpContextAccessor.HttpContext?.Request.Host.ToString() ?? "localhost";
            var port = _httpContextAccessor.HttpContext?.Request.Host.Port ?? 5000;
            var pathBase = filePath.Replace(_storagePath, "");

            var finalUrl = $"{scheme}://{host}:{port}{pathBase}";
            return Result<string>.Create(ResponseStatusCode.Success, finalUrl);
        }

        private static string GenerateFileName() => Guid.NewGuid().ToString();
    }
}

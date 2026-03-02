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
            if (request.ImageStream == null || request.ImageStream.Length == 0)
            {
                return Result<string>.Create(
                    ResponseStatusCode.BadRequest,
                    null,
                    "Invalid image stream"
                );
            }

            var fullfolderPath = Path.Combine(_storagePath, folder);
            Directory.CreateDirectory(fullfolderPath);

            var fileName = GenerateFileName();
            var filePath = Path.Combine(
                fullfolderPath,
                $"{fileName}.{request.Format.ToString().ToLower()}"
            );

            var spec = ImageProcessor.BuildSpec(request);

            var image = await ImageSharpAdapter.ProcessAsync(request.ImageStream, spec);
            await image.SaveAsync(filePath);

            var finalUrl = BuildPublicUrl(filePath);
            return Result<string>.Create(ResponseStatusCode.Success, finalUrl);
        }

        private string BuildPublicUrl(string filePath)
        {
            var request = _httpContextAccessor.HttpContext?.Request;
            var scheme = request?.Scheme ?? "http";
            var host = request?.Host.ToString() ?? "localhost";
            var port = request?.Host.Port ?? 5000;

            var relativePath = filePath.Replace(_storagePath, "").Replace("\\", "/");
            return $"{scheme}://{host}:{port}{relativePath}";
        }

        private static string GenerateFileName() => Guid.NewGuid().ToString();
    }
}

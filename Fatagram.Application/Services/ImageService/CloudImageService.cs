using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Fatagram.Application.Services.ImageService.Interface;
using Fatagram.Application.Utils;
using Fatagram.Shared.Enums;
using Microsoft.Extensions.Configuration;

namespace Fatagram.Application.Services.ImageService
{
    public class CloudImageService(Cloudinary cloudinary, IConfiguration config) : IImageService
    {
        private readonly Cloudinary _cloudinary = cloudinary;
        private readonly IConfiguration _config = config;

        public Task<Result<object>> GetUploadSignatureAsync()
        {
            var testSecret = _config["CloudStorage:Cloudinary:ApiSecret"];
            Console.WriteLine($"\n[TEST SECRET] Tui đang đọc cái Secret là: '{testSecret}'\n");

            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var parameters = new Dictionary<string, object>
            {
                { "folder", "chat_messages" },
                { "overwrite", "false" },
                { "timestamp", timestamp.ToString() },
            };

            string signature = _cloudinary.Api.SignParameters(parameters);

            var result = new
            {
                apiKey = _config["CloudStorage:Cloudinary:ApiKey"],
                cloudName = _config["CloudStorage:Cloudinary:CloudName"],
                timestamp,
                folder = "chat_messages",
                overwrite = "false",
                signature,
            };

            return Task.FromResult(Result<object>.Create(ResponseStatusCode.Success, result));
        }

        public async Task<Result<string>> SaveImageAsync(ImageRequest request, string folder)
        {
            if (_cloudinary == null)
            {
                Console.WriteLine("Cloudinary client is not initialized.");
            }
            Console.WriteLine($"Received image request: {request}, Folder: {folder}");
            if (request == null || request.ImageStream == null || request.ImageStream.Length == 0)
            {
                return Result<string>.Create(
                    ResponseStatusCode.BadRequest,
                    null,
                    "Invalid image request"
                );
            }
            // Set real size for original preset if width or height is not provided

            request.ImageStream.Position = 0;
            var spec = ImageProcessor.BuildSpec(request);

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription("image", request.ImageStream),
                Folder = folder,
                Transformation = CloudinaryAdapter.BuildTransformation(spec),
            };

            var result = await _cloudinary!.UploadAsync(uploadParams);

            // Write message from cloudinary response to console for debugging
            Console.WriteLine(
                $"Cloudinary upload result: {result.StatusCode}, Message: {result.Error?.Message}"
            );

            return Result<string>.Create(ResponseStatusCode.Success, result.SecureUrl.ToString());
        }
    }
}

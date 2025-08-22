using Fatagram.Application.Services.ImageService.Interface;
using Fatagram.Application.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;
using Fatagram.Application.Services.ImageService.Enum;
using Fatagram.Shared.Extensions;

namespace Fatagram.Application.Services.ImageService
{
    public class WwwRootImageService : IImageService
    {
        private readonly string _webRootPath;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public WwwRootImageService(IConfiguration config, IHttpContextAccessor httpContextAccessor)
        {
            _webRootPath = config["WwwRootPath"] ?? throw new ArgumentNullException(nameof(config));
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Result<string>> SaveImageAsync(
            ImageSize size, 
            Stream imageStream, 
            string fileExtension, 
            string folder,
            bool isAvatar = false)
        {
            var folderPath = Path.Combine(_webRootPath, folder);
            Directory.CreateDirectory(folderPath);

            var fileName = $"{Guid.NewGuid()}.jpg"; // Lưu luôn thành .jpg để đồng bộ format
            var filePath = Path.Combine(folderPath, fileName);

            using (var image = await Image.LoadAsync(imageStream)) // Tự detect format
            {
                int minSize = Math.Min(image.Width, image.Height);
                var cropRectangle = new Rectangle(
                    (image.Width - minSize) / 2,
                    (image.Height - minSize) / 2,
                    minSize,
                    minSize
                );
                
                image.Mutate(x =>
                {
                    if (isAvatar) x.Crop(cropRectangle);

                    if (size == ImageSize.Small)
                    {
                        x.Resize(new ResizeOptions
                        {
                            Mode = ResizeMode.Max,
                            Size = new Size(480, 480)
                        });
                    }
                    else if (size == ImageSize.Medium)
                    {
                        x.Resize(new ResizeOptions
                        {
                            Mode = ResizeMode.Max,
                            Size = new Size(720, 720)
                        });
                    }
                    else if (size == ImageSize.Large)
                    {
                        x.Resize(new ResizeOptions
                        {
                            Mode = ResizeMode.Max,
                            Size = new Size(1024, 1024)
                        });
                    }
                });


                // Nén chất lượng ảnh (chọn 70-80 là ổn)
                var encoder = new JpegEncoder { Quality = 75 };

                await image.SaveAsJpegAsync(filePath, encoder);
            }

            var request = _httpContextAccessor.HttpContext?.Request;
            if (request == null)
                return Result<string>.BadRequest("Request context is not available.");

            var baseUrl = $"http://192.168.137.1:5002";
            return Result<string>.Success($"{baseUrl}/{folder}/{fileName}");
        }
    }
}

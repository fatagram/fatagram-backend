using Fatagram.Application.Services.ImageService.Interface;
using Fatagram.Shared.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;
using Fatagram.Application.Services.ImageService.Enum;


namespace Fatagram.Application.Services.ImageService
{
    public class WwwrootImageService : IImageService
    {
        private readonly string _webRootPath;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public WwwrootImageService(IConfiguration config, IHttpContextAccessor httpContextAccessor)
        {
            _webRootPath = config["WwwRootPath"] ?? throw new ArgumentNullException(nameof(config));
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<Result<string>> SaveImageAsync(ImageSize size, Stream imageStream, string fileExtension, string folder)
        {
            try
            {
                var folderPath = Path.Combine(_webRootPath, folder);
                Directory.CreateDirectory(folderPath);

                var fileName = $"{Guid.NewGuid()}.jpg"; // Lưu luôn thành .jpg để đồng bộ format
                var filePath = Path.Combine(folderPath, fileName);

                using (var image = await Image.LoadAsync(imageStream)) // Tự detect format
                {
                    if (size == ImageSize.Small)
                    {
                        image.Mutate(x => x.Resize(new ResizeOptions
                        {
                            Mode = ResizeMode.Max,
                            Size = new Size(480, 480)
                        }));
                    }
                    else if (size == ImageSize.Medium)
                    {
                        image.Mutate(x => x.Resize(new ResizeOptions
                        {
                            Mode = ResizeMode.Max,
                            Size = new Size(720, 720)
                        }));
                    }
                    else if (size == ImageSize.Large)
                    {
                        image.Mutate(x => x.Resize(new ResizeOptions
                        {
                            Mode = ResizeMode.Max,
                            Size = new Size(1024, 1024)
                        }));
                    }

                    // Nén chất lượng ảnh (chọn 70-80 là ổn)
                    var encoder = new JpegEncoder { Quality = 75 };

                    await image.SaveAsJpegAsync(filePath, encoder);
                }

                var request = _httpContextAccessor.HttpContext?.Request;
                if (request == null)
                    return Result<string>.Failure("Request context is not available.");

                var baseUrl = $"{request.Scheme}://{request.Host.Value}";
                return Result<string>.Success($"{baseUrl}/{folder}/{fileName}");
            }
            catch (Exception ex)
            {
                return Result<string>.Failure(ex.Message);
            }
        }
    }
}

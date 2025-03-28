using Fatagram.Application.Services.ImageService.Interface;
using Fatagram.Shared.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

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

        public async Task<Result<string>> SaveImageAsync(Stream imageStream, string fileExtension, string folder)
        {
            try
            {
                var folderPath = Path.Combine(_webRootPath, folder);
                Directory.CreateDirectory(folderPath);

                var fileName = $"{Guid.NewGuid()}{fileExtension}";
                var filePath = Path.Combine(folderPath, fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await imageStream.CopyToAsync(fileStream);
                }

                // Lấy BaseUrl động theo request
                var request = _httpContextAccessor.HttpContext?.Request;
                if (request == null)
                    return Result<string>.Failure("Request context is not available.");

                var baseUrl = $"{request.Scheme}://{request.Host.Value}";

                // Trả về đường dẫn tuyệt đối
                return Result<string>.Success($"{baseUrl}/{folder}/{fileName}");
            }
            catch (Exception ex)
            {
                return Result<string>.Failure(ex.Message);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.API.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Fatagram.API.Controllers.V1
{
    public class UploadController(IImageService imageService, ILogger<UploadController> logger)
        : BaseApiController
    {
        private readonly IImageService _imageService = imageService;
        private readonly ILogger<UploadController> _logger = logger;

        [HttpGet("signature")]
        public async Task<IActionResult> GetUploadSignature()
        {
            var result = await _imageService.GetUploadSignatureAsync();
            return result.ToActionResult();
        }
    }
}

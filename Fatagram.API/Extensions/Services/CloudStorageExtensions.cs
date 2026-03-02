using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CloudinaryDotNet;

namespace Fatagram.API.Extensions.Services
{
    public static class CloudStorageExtension
    {
        public static void AddCloudStorageServices(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            var provider = configuration["CloudStorage:Provider"];

            if (provider == "Cloudinary")
            {
                var section = configuration.GetSection("CloudStorage:Cloudinary");

                var account = new Account(
                    section["CloudName"],
                    section["ApiKey"],
                    section["ApiSecret"]
                );

                services.AddSingleton(new Cloudinary(account));
            }
            else
            {
                throw new InvalidOperationException(
                    $"Unsupported CloudStorage provider: {provider}"
                );
            }
        }
    }
}

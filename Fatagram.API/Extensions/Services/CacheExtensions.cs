using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.Infrastructure.Cache;

namespace Fatagram.API.Extensions.Services
{
    public static class CacheExtensions
    {
        public static void AddCacheServices(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            var redisConnectionString = configuration.GetConnectionString("Redis");

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConnectionString;
                options.InstanceName = "Fatagram_";
            });

            services.AddSingleton<ICacheService, RedisCacheService>();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.Application.Abstractions.Cache;
using Fatagram.Infrastructure.Cache;
using StackExchange.Redis;

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
            services.AddSingleton<IConnectionMultiplexer>(sp =>
                ConnectionMultiplexer.Connect(redisConnectionString!)
            );

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConnectionString;
                options.InstanceName = "Fatagram_";
            });

            services.AddSingleton<ICacheService, RedisCacheService>();
        }
    }
}

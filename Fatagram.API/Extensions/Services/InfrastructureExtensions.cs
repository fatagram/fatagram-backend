using CloudinaryDotNet;
using Fatagram.Application.Abstractions.Cache;
using Fatagram.Infrastructure.Cache;
using Npgsql;
using StackExchange.Redis;

namespace Fatagram.API.Extensions.Services
{
    public static class InfrastructureExtensions
    {
        public static void AddDatabaseServices(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
            dataSourceBuilder.EnableDynamicJson();
            var dataSource = dataSourceBuilder.Build();
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseNpgsql(dataSource);
            });
        }

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

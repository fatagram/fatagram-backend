using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.Infrastructure.Cache;
using Fatagram.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Fatagram.Infrastructure.Workers
{
    public class SyncUserLastMessageSequenceNumber : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<SyncUserLastMessageSequenceNumber> _logger;

        public SyncUserLastMessageSequenceNumber(
            IServiceScopeFactory scopeFactory,
            ILogger<SyncUserLastMessageSequenceNumber> logger
        )
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        private class CacheSeenData
        {
            public Guid MessageId { get; set; }
            public DateTime SeenAt { get; set; }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                Console.WriteLine("Sync Seen Status đang chạy lúc: " + DateTime.Now);
                try
                {
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var cache = scope.ServiceProvider.GetRequiredService<ICacheService>();
                        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                        var seenKeys = await cache.GetKeysAsync("user:*:last_read");
                        foreach (var key in seenKeys)
                        {
                            var userId = Guid.Parse(key.Split(':')[1]);
                            Console.WriteLine(
                                $"Đang Sync Last Read Sequence Number cho UserId: {userId}"
                            );

                            var lastReadData = await cache.HashGetAllAsync(key);

                            foreach (var entry in lastReadData)
                            {
                                var convId = Guid.Parse(entry.Key);
                                var seqNumber = int.Parse(entry.Value);

                                await db
                                    .ConversationParticipants.Where(cp =>
                                        cp.ConversationId == convId && cp.UserId == userId
                                    )
                                    .ExecuteUpdateAsync(s =>
                                        s.SetProperty(cp => cp.LastSeenNumber, seqNumber)
                                    );
                            }
                        }
                    }
                    _logger.LogInformation("Sync Seen Status thành công lúc: {time}", DateTime.Now);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Lỗi khi đang Sync Seen Data!");
                }

                await Task.Delay(TimeSpan.FromHours(5), stoppingToken);
            }
        }
    }
}

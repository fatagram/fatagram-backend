using Fatagram.Infrastructure.Cache;
using Fatagram.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class SyncSeenWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<SyncSeenWorker> _logger;

    public SyncSeenWorker(IServiceScopeFactory scopeFactory, ILogger<SyncSeenWorker> logger)
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
                    var seenKeys = await cache.GetKeysAsync("conv:*:seen");

                    foreach (var key in seenKeys)
                    {
                        var convId = Guid.Parse(key.Split(':')[1]);
                        Console.WriteLine($"Đang Sync Seen Status cho ConversationId: {convId}");

                        var seenData = await cache.HashGetAllAsync(key);

                        foreach (var entry in seenData)
                        {
                            var userId = Guid.Parse(entry.Key);

                            var parsedData =
                                System.Text.Json.JsonSerializer.Deserialize<CacheSeenData>(
                                    entry.Value
                                );

                            if (parsedData != null)
                            {
                                await db
                                    .ConversationParticipants.Where(cp =>
                                        cp.ConversationId == convId && cp.UserId == userId
                                    )
                                    .ExecuteUpdateAsync(s =>
                                        s.SetProperty(
                                                p => p.LastSeenMessageId,
                                                parsedData.MessageId
                                            )
                                            .SetProperty(p => p.SeenAt, parsedData.SeenAt) // Update cả SeenAt vào DB (Update SeenAt to DB too)
                                    );
                            }
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

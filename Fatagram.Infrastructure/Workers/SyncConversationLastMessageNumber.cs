using Fatagram.Infrastructure.Cache;
using Fatagram.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class SyncConversationLastMessageNumber : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<SyncConversationLastMessageNumber> _logger;

    public SyncConversationLastMessageNumber(
        IServiceScopeFactory scopeFactory,
        ILogger<SyncConversationLastMessageNumber> logger
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
                    var metaKeys = await cache.GetKeysAsync("conv:*:meta");
                    foreach (var key in metaKeys)
                    {
                        var keyParts = key.Split(':');
                        if (keyParts.Length < 2 || !Guid.TryParse(keyParts[1], out var convId))
                            continue;

                        var maxSeqStr = await cache.HashGetAsync(key, "max_seq");

                        if (
                            !string.IsNullOrEmpty(maxSeqStr)
                            && int.TryParse(maxSeqStr, out var maxSeq)
                        )
                        {
                            await db
                                .Conversations.Where(c => c.Id == convId)
                                .ExecuteUpdateAsync(
                                    s => s.SetProperty(p => p.LastMessageNumber, maxSeq),
                                    stoppingToken
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

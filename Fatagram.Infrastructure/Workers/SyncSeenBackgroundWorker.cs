using System.Text.Json;
using Fatagram.Infrastructure.Cache;
using Fatagram.Infrastructure.Data;
using Fatagram.Infrastructure.Projections;
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

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var cache = scope.ServiceProvider.GetRequiredService<ICacheService>();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var lastReadKeys = await cache.GetKeysAsync("user:*:last_read");

                foreach (var key in lastReadKeys)
                {
                    if (!Guid.TryParse(key.Split(':')[1], out var userId))
                        continue;

                    var lastReadData = await cache.HashGetAllAsync(key);

                    foreach (var entry in lastReadData)
                    {
                        if (!Guid.TryParse(entry.Key, out var convId))
                            continue;

                        try
                        {
                            var info = JsonSerializer.Deserialize<ParticipantSeenInfoProjection>(
                                entry.Value
                            );

                            if (info != null)
                            {
                                await db
                                    .ConversationParticipants.Where(cp =>
                                        cp.ConversationId == convId && cp.UserId == userId
                                    )
                                    .ExecuteUpdateAsync(
                                        s =>
                                            s.SetProperty(
                                                    p => p.LastSeenNumber,
                                                    info.SequenceNumber
                                                )
                                                .SetProperty(p => p.SeenAt, info.SeenAt),
                                        stoppingToken
                                    );
                            }
                        }
                        catch (JsonException) { }
                    }
                }
                _logger.LogInformation("Sync Seen Data (Sequence & Actual Time) completed.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SyncSeenWorker error!");
            }

            await Task.Delay(TimeSpan.FromMinutes(15), stoppingToken);
        }
    }
}

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

                    // 1. Sync (Đồng bộ) trạng thái đã xem (MessageId và SeenAt)
                    var seenKeys = await cache.GetKeysAsync("conv:*:seen");

                    foreach (var key in seenKeys)
                    {
                        var keyParts = key.Split(':');
                        // Skip (Bỏ qua) nếu key (từ khóa) không hợp lệ
                        if (keyParts.Length < 2 || !Guid.TryParse(keyParts[1], out var convId))
                            continue;

                        Console.WriteLine(
                            $"Đang Sync (Đồng bộ) Seen Status cho ConversationId: {convId}"
                        );

                        var seenData = await cache.HashGetAllAsync(key);

                        foreach (var entry in seenData)
                        {
                            if (!Guid.TryParse(entry.Key, out var userId))
                                continue;

                            var parsedData =
                                System.Text.Json.JsonSerializer.Deserialize<CacheSeenData>(
                                    entry.Value
                                );

                            if (parsedData != null)
                            {
                                await db
                                    .ConversationParticipants.Where(cp =>
                                        cp.ConversationId == convId
                                        && cp.UserId == userId
                                        // Check (Kiểm tra) tin nhắn phải tồn tại để tránh lỗi Foreign Key (Khóa ngoại)
                                        && db.Messages.Any(m => m.Id == parsedData.MessageId)
                                    )
                                    .ExecuteUpdateAsync(
                                        s =>
                                            s.SetProperty(
                                                    p => p.LastSeenMessageId,
                                                    parsedData.MessageId
                                                )
                                                .SetProperty(p => p.SeenAt, parsedData.SeenAt),
                                        stoppingToken
                                    );
                            }
                        }
                    }

                    // 2. Sync (Đồng bộ) sequence number (số thứ tự) để count (đếm) unread messages (tin nhắn chưa đọc)
                    var unreadKeys = await cache.GetKeysAsync("user:*:last_read");

                    foreach (var key in unreadKeys)
                    {
                        var keyParts = key.Split(':');
                        if (keyParts.Length < 2 || !Guid.TryParse(keyParts[1], out var userId))
                            continue;

                        var userLastReadData = await cache.HashGetAllAsync(key);

                        foreach (var entry in userLastReadData)
                        {
                            if (
                                !Guid.TryParse(entry.Key, out var convId)
                                || !int.TryParse(entry.Value, out var sequenceNumber)
                            )
                                continue;

                            await db
                                .ConversationParticipants.Where(cp =>
                                    cp.ConversationId == convId && cp.UserId == userId
                                )
                                .ExecuteUpdateAsync(
                                    s => s.SetProperty(p => p.LastSeenNumber, sequenceNumber),
                                    stoppingToken
                                );
                        }
                    }

                    // 3. Sync (Đồng bộ) total messages (tổng số lượng tin nhắn) của conversation (phòng trò chuyện)
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
                _logger.LogError(ex, "Error (Lỗi) khi đang Sync (Đồng bộ) Seen Data!");
            }

            // Delay (Trì hoãn) 5 tiếng trước khi chạy lại loop (vòng lặp)
            await Task.Delay(TimeSpan.FromHours(5), stoppingToken);
        }
    }
}

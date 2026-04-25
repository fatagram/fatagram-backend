using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.Domain.Models;
using Fatagram.Infrastructure.Cache;
using Fatagram.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Fatagram.Infrastructure.Workers
{
    public class BatchingMessagesWorker(
        IServiceScopeFactory serviceScopeFactory,
        ILogger<BatchingMessagesWorker> logger
    ) : BackgroundService
    {
        private IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;
        private ILogger<BatchingMessagesWorker> _logger = logger;

        private const int BatchSize = 50;
        private readonly TimeSpan _batchInterval = TimeSpan.FromMinutes(1);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("BatchingMessagesWorker is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation(
                    "BatchingMessagesWorker is running at: {time}",
                    DateTimeOffset.Now
                );

                try
                {
                    await ProcessBatchAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while batching messages.");
                }

                try
                {
                    await Task.Delay(_batchInterval, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }

        private async Task ProcessBatchAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var cacheService = scope.ServiceProvider.GetRequiredService<ICacheService>();

            var queueKeys = await cacheService.GetKeysAsync("conv:*:messages");

            foreach (var key in queueKeys)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                var pendingMessages = await cacheService.SortedSetRangeByScoreAsync<Message>(
                    key,
                    order: Order.Ascending,
                    take: BatchSize
                );

                if (pendingMessages.Count == 0)
                    continue;

                var strategy = dbContext.Database.CreateExecutionStrategy();

                await strategy.ExecuteAsync(async () =>
                {
                    using var transaction = await dbContext.Database.BeginTransactionAsync(
                        cancellationToken
                    );

                    try
                    {
                        var conversationId = pendingMessages.First().ConversationId;

                        // ── 1. Dedup trong batch: trùng SequenceNumber giữ cái UpdatedAt mới nhất ──
                        var dedupedMessages = pendingMessages
                            .GroupBy(m => new { m.ConversationId, m.SequenceNumber })
                            .Select(g => g.OrderByDescending(m => m.UpdatedAt).First())
                            .OrderBy(m => m.CreatedAt)
                            .ToList();

                        // ── 2. Loại Id và SequenceNumber đã có trong DB ──
                        var incomingIds = dedupedMessages.Select(m => m.Id).ToList();
                        var incomingSeqs = dedupedMessages.Select(m => m.SequenceNumber).ToList();

                        var existingIds = await dbContext
                            .Messages.Where(m => incomingIds.Contains(m.Id))
                            .Select(m => m.Id)
                            .ToHashSetAsync(cancellationToken);

                        var existingSeqs = await dbContext
                            .Messages.Where(m =>
                                m.ConversationId == conversationId
                                && incomingSeqs.Contains(m.SequenceNumber)
                            )
                            .Select(m => m.SequenceNumber)
                            .ToHashSetAsync(cancellationToken);

                        var filteredMessages = dedupedMessages
                            .Where(m =>
                                !existingIds.Contains(m.Id)
                                && !existingSeqs.Contains(m.SequenceNumber)
                            )
                            .ToList();

                        // ── 3. Nếu không còn gì mới → chỉ xóa cache rồi bỏ qua ──
                        if (filteredMessages.Count == 0)
                        {
                            _logger.LogWarning(
                                "All messages in batch for key {key} already exist in DB. Cleaning cache.",
                                key
                            );
                            foreach (var msg in pendingMessages)
                                await cacheService.SortedSetRemoveAsync(key, msg);

                            await transaction.CommitAsync(cancellationToken);
                            return;
                        }

                        // ── 4. Reassign SequenceNumber liên tục tránh gap / trùng ──
                        var lastSeq =
                            await dbContext
                                .Messages.Where(m => m.ConversationId == conversationId)
                                .MaxAsync(m => (long?)m.SequenceNumber, cancellationToken) ?? 0;

                        var resequenced = filteredMessages
                            .Select(
                                (msg, idx) =>
                                {
                                    msg.SequenceNumber = (int)(lastSeq + idx + 1);
                                    return msg;
                                }
                            )
                            .ToList();

                        // ── 5. Map sang entity mới để tránh EF tracking conflict ──
                        var messagesToPersist = resequenced
                            .Select(msg => new Message
                            {
                                Id = msg.Id,
                                ConversationId = msg.ConversationId,
                                SenderId = msg.SenderId,
                                Content = msg.Content,
                                ReadAt = msg.ReadAt,
                                Type = msg.Type,
                                Metadata = msg.Metadata,
                                SequenceNumber = msg.SequenceNumber,
                                CreatedAt = msg.CreatedAt,
                                UpdatedAt = msg.UpdatedAt,
                                DeletedAt = msg.DeletedAt,
                                Version = msg.Version,
                                Media = msg
                                    .Media?.Select(media => new MessageMedia
                                    {
                                        Id = media.Id,
                                        MessageId = media.MessageId,
                                        Url = media.Url,
                                        Type = media.Type,
                                        Metadata = media.Metadata,
                                        CreatedAt = media.CreatedAt,
                                        UpdatedAt = media.UpdatedAt,
                                        DeletedAt = media.DeletedAt,
                                        Version = media.Version,
                                        IndexInMessage = media.IndexInMessage,
                                        MessageSequence = msg.SequenceNumber, // sync với seq mới
                                    })
                                    .ToList(),
                            })
                            .ToList();

                        await dbContext.Messages.AddRangeAsync(
                            messagesToPersist,
                            cancellationToken
                        );

                        // ── 6. Cập nhật LastMessageNumber theo sequence thực tế sau reassign ──
                        await dbContext
                            .Conversations.Where(c => c.Id == conversationId)
                            .ExecuteUpdateAsync(
                                s =>
                                    s.SetProperty(
                                        c => c.LastMessageNumber,
                                        resequenced.Last().SequenceNumber
                                    ),
                                cancellationToken
                            );

                        await dbContext.SaveChangesAsync(cancellationToken);

                        // ── 7. Commit DB trước, xóa cache sau ──
                        await transaction.CommitAsync(cancellationToken);

                        foreach (var message in pendingMessages)
                            await cacheService.SortedSetRemoveAsync(key, message);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to process message batch for key: {key}", key);
                        await transaction.RollbackAsync(cancellationToken);
                    }
                });
            }
        }
    }
}

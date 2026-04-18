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
                        var messagesToPersist = pendingMessages
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
                                        MessageSequence = media.MessageSequence,
                                    })
                                    .ToList(),
                            })
                            .ToList();

                        await dbContext.Messages.AddRangeAsync(
                            messagesToPersist,
                            cancellationToken
                        );
                        await dbContext
                            .Conversations.Where(c =>
                                c.Id == pendingMessages.First().ConversationId
                            )
                            .ExecuteUpdateAsync(
                                s =>
                                    s.SetProperty(
                                        c => c.LastMessageNumber,
                                        pendingMessages.Last().SequenceNumber
                                    ),
                                cancellationToken
                            );
                        await dbContext.SaveChangesAsync(cancellationToken);

                        foreach (var message in pendingMessages)
                        {
                            await cacheService.SortedSetRemoveAsync(key, message);
                        }

                        await transaction.CommitAsync(cancellationToken);
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

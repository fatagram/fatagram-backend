using System.Text.Json;
using Fatagram.Infrastructure.MessageQueue.Interfaces;
using StackExchange.Redis;

namespace Fatagram.Infrastructure.MessageQueue
{
    public class RedisMessageQueue(IConnectionMultiplexer redis) : IMessageQueue
    {
        private readonly IConnectionMultiplexer _redis = redis;
        private readonly IDatabase _db = redis.GetDatabase();
        private const string DelayQueueSuffix = ":delayed";

        public async Task PublishAsync<T>(
            string queueName,
            T message,
            CancellationToken cancellationToken = default
        )
            where T : class
        {
            if (string.IsNullOrWhiteSpace(queueName))
                throw new ArgumentException("Queue name cannot be empty.", nameof(queueName));

            if (message == null)
            {
                throw new ArgumentException(nameof(message));
            }

            string payload = JsonSerializer.Serialize(message);
            await _db.ListLeftPushAsync(queueName, payload);
        }

        public async Task PublishDelayedAsync<T>(
            string queueName,
            T message,
            TimeSpan delay,
            CancellationToken cancellationToken = default
        )
            where T : class
        {
            if (string.IsNullOrWhiteSpace(queueName))
                throw new ArgumentException("Queue name cannot be empty.", nameof(queueName));

            if (message == null)
                throw new ArgumentNullException(nameof(message));

            string payload = JsonSerializer.Serialize(message);

            double executeAtTimestamp = DateTimeOffset.UtcNow.Add(delay).ToUnixTimeSeconds();

            string delayedQueueName = $"{queueName}{DelayQueueSuffix}";

            await _db.SortedSetAddAsync(delayedQueueName, payload, executeAtTimestamp);
        }

        public void Subscribe<T>(string queueName, Func<T, CancellationToken, Task> handler)
        {
            if (string.IsNullOrWhiteSpace(queueName))
                throw new ArgumentException("Queue name cannot be empty.", nameof(queueName));

            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            // Task.Run(async () =>
            // {
            //     string delayedQueueName = $"{queueName}{DelayQueueSuffix}";

            //     while (true) { }
            // });
        }
    }
}

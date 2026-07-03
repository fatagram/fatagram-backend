using System;
using System.Threading;
using System.Threading.Tasks;

namespace Fatagram.Application.Abstractions.MessageQueue
{
    public interface IMessageQueue
    {
        /// <summary>
        /// Pushes a message into a specific queue.
        /// </summary>
        /// <typeparam name="T">The type of the message payload.</typeparam>
        /// <param name="queueName">The target queue destination.</param>
        /// <param name="message">The actual data payload to be processed.</param>
        Task PublishAsync<T>(
            string queueName,
            T message,
            CancellationToken cancellationToken = default
        )
            where T : class;

        /// <summary>
        /// Pushes a message into a specific queue with a scheduled delay time.
        /// </summary>
        /// <param name="delay">The duration to wait before the message becomes available for consumption.</param>
        Task PublishDelayedAsync<T>(
            string queueName,
            T message,
            TimeSpan delay,
            CancellationToken cancellationToken = default
        )
            where T : class;

        /// <summary>
        /// Subscribes a consumer logic to process messages arriving at a specific queue.
        /// </summary>
        /// <typeparam name="T">The expected type of the message payload.</typeparam>
        /// <param name="queueName">The queue to listen to.</param>
        /// <param name="handler">The asynchronous delegate function that executes the business logic.</param>
        void Subscribe<T>(string queueName, Func<T, CancellationToken, Task> handler);
    }
}

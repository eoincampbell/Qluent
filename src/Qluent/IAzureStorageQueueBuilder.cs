namespace Qluent
{
    using Policies.PoisonMessageBehavior;
    using Serialization;
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Fluent API Builder
    /// </summary>
    /// <typeparam name="T">The object type</typeparam>
    /// <seealso cref="Qluent.IAzureStorageQueueBuilder{T}" />
    public interface IAzureStorageQueueBuilder<T>
    {
        /// <summary>
        /// Builds an instance of an <see cref="IAzureStorageQueue{T}"/>
        /// </summary>
        /// <returns>An instance of the queue</returns>
        IAzureStorageQueue<T> Build();
        /// <summary>
        /// Asynchronoulsy builds an instance of an <see cref="IAzureStorageQueue{T}"/>
        /// </summary>
        /// <returns>An instance of the queue</returns>
        Task<IAzureStorageQueue<T>> BuildAsync();
        /// <summary>
        /// Fluently specify the account you want to connect to
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>This builder instance</returns>
        IAzureStorageQueueBuilder<T> ConnectedToAccount(string connectionString);
        /// <summary>
        /// Fluently specify the name of the storage queue
        /// </summary>
        /// <param name="storageQueueName">Name of the storage queue.</param>
        /// <returns>This builder instance</returns>
        IAzureStorageQueueBuilder<T> UsingStorageQueue(string storageQueueName);
        /// <summary>
        /// Fluently specify a custom serializer to convert your object to a <see cref="T:System.Byte[]"/>
        /// </summary>
        /// <param name="customSerlializer">The custom serlializer.</param>
        /// <returns>This builder instance</returns>
        IAzureStorageQueueBuilder<T> WithACustomSerializer(IStringMessageSerializer<T> customSerlializer);
        /// <summary>
        /// Fluently specify a custom serializer to convert your object to a <see cref="T:System.Byte[]"/>
        /// </summary>
        /// <param name="customSerlializer">The custom serlializer.</param>
        /// <returns>This builder instance</returns>
        IAzureStorageQueueBuilder<T> WithACustomSerializer(IBinaryMessageSerializer<T> customSerlializer);
        /// <summary>
        /// Fluently specify the dequeue threshold to consider messages poison
        /// </summary>
        /// <param name="dequeueAttempts">The dequeue attempts.</param>
        /// <returns>This builder instance</returns>
        IAzureStorageQueueBuilder<T> ThatConsidersMessagesPoisonAfter(int dequeueAttempts);
        /// <summary>
        /// Fluently specify the name of the poison queue to route poison messages to
        /// </summary>
        /// <param name="poisonQueueName">Name of the poison queue.</param>
        /// <returns>This builder instance</returns>
        IAzureStorageQueueBuilder<T> AndSendsPoisonMessagesTo(string poisonQueueName);
        /// <summary>
        /// Fluently specify the exception handling behavior for poison messages
        /// </summary>
        /// <param name="behavior">The behavior.</param>
        /// <returns>This builder instance</returns>
        IAzureStorageQueueBuilder<T> AndHandlesExceptionsOnPoisonMessages(By behavior);
        /// <summary>
        /// Fluently specify initial visibility delay for messages to appear on the queue
        /// </summary>
        /// <param name="timespan">The timespan.</param>
        /// <returns>This builder instance</returns>
        IAzureStorageQueueBuilder<T> ThatDelaysMessageVisibilityAfterEnqueuingFor(TimeSpan timespan);
        /// <summary>
        /// Fluently specify time a message will remain invisible after dequeuing
        /// </summary>
        /// <param name="timespan">The timespan.</param>
        /// <returns>This builder instance</returns>
        IAzureStorageQueueBuilder<T> ThatKeepsMessagesInvisibleAfterDequeuingFor(TimeSpan timespan);
        /// <summary>
        /// Fluently specify the TTL of a message
        /// </summary>
        /// <param name="timespan">The timespan.</param>
        /// <returns>This builder instance</returns>
        IAzureStorageQueueBuilder<T> ThatSetsAMessageTtlOf(TimeSpan timespan);
    }
}
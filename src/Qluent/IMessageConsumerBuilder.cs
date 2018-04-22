using System.Threading;
using System.Threading.Tasks;

namespace Qluent
{
    using System;
    using Consumers.Handlers;
    using Consumers.Policies;

    /// <summary>
    /// Fluent API Builder for creating an <see cref="IMessageConsumer{T}" />
    /// </summary>
    /// <typeparam name="T">The object type of the queue's message payload</typeparam>
    public interface IMessageConsumerBuilder<T>
    {
        /// <summary>
        /// Builds an instance of an <see cref="IMessageConsumer{T}"/>
        /// </summary>
        /// <returns>An instance of the consumer</returns>
        IMessageConsumer<T> Build();
        /// <summary>
        /// Fluently specifiy the queue to have this consumer connect to
        /// </summary>
        /// <param name="queue">The queue.</param>
        /// <returns>This builde instance</returns>
        IMessageConsumerBuilder<T> UsingQueue(IAzureStorageQueue<T> queue);
        /// <summary>
        /// Fluently specifiy the handler for processing a message
        /// </summary>
        /// <param name="messageHandler">The message handler.</param>
        /// <returns>This builde instance</returns>
        IMessageConsumerBuilder<T> ThatHandlesMessagesUsing(IMessageHandler<T> messageHandler);
        /// <summary>
        /// Fluently specifiy the handler for a message when processing fails
        /// </summary>
        /// <param name="failedMessagehandler">The failed messagehandler.</param>
        /// <returns>This builde instance</returns>
        IMessageConsumerBuilder<T> AndHandlesFailedMessagesUsing(IMessageHandler<T> failedMessagehandler);
        /// <summary>
        /// Fluently specifiy the handler for scenarios when processing causes an exception
        /// </summary>
        /// <param name="messageExceptionHandler">The message exception handler.</param>
        /// <returns>This builde instance</returns>
        IMessageConsumerBuilder<T> AndHandlesExceptionsUsing(IMessageExceptionHandler<T> messageExceptionHandler);
        /// <summary>
        /// Fluently specifiy the handler for processing a message
        /// </summary>
        /// <param name="messageHandler">The message handler.</param>
        /// <returns>This builde instance</returns>
        IMessageConsumerBuilder<T> ThatHandlesMessagesUsing(Func<IMessage<T>, CancellationToken, Task<bool>> messageHandler);
        /// <summary>
        /// Fluently specifiy the handler for a message when processing fails 
        /// </summary>
        /// <param name="failedMessagehandler">The failed messagehandler.</param>
        /// <returns>This builde instance</returns>
        IMessageConsumerBuilder<T> AndHandlesFailedMessagesUsing(Func<IMessage<T>, CancellationToken, Task<bool>> failedMessagehandler);
        /// <summary>
        /// Fluently specifiy the handler for scenarios when processing causes an exception
        /// </summary>
        /// <param name="messageExceptionHandler">The message exception handler.</param>
        /// <returns>This builde instance</returns>
        IMessageConsumerBuilder<T> AndHandlesExceptionsUsing(Func<IMessage<T>, Exception, CancellationToken, Task<bool>> messageExceptionHandler);
        /// <summary>
        /// Fluently specifiy the QueuePollingPolicy to be used
        /// </summary>
        /// <param name="policy">The policy.</param>
        /// <returns>This builde instance</returns>
        IMessageConsumerBuilder<T> WithAQueuePolingPolicyOf(IMessageConsumerQueuePolingPolicy policy);


    }
}
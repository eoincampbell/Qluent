namespace Qluent.Consumers.Handlers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// An interface specifying how the the consumer should handle an <see cref="IMessage{T}"/> 
    /// which has been dequeued if processing of the message resulted in an exception.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IMessageExceptionHandler<T>
    {
        /// <summary>
        /// Handles the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="ex">The exception.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A <see cref="Task"/> object that represents the asynchronous operation.
        ///     
        /// The task result contains whether the message was handled successfully or not.
        /// </returns>
        Task<bool> Handle(IMessage<T> message, Exception ex, CancellationToken cancellationToken);
    }
}
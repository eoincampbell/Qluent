namespace Qluent.Consumers.Handlers
{
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// An interface specifying how the the consumer should handle an <see cref="IMessage{T}"/> which has been dequeued
    /// </summary>
    /// <typeparam name="T">The payload type of the message</typeparam>
    public interface IMessageHandler<T>
    {
        /// <summary>
        /// Handles the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A <see cref="Task{bool}"/> object that represents the asynchronous operation.
        ///     
        /// The task result contains whether the message was handled successfully or not.
        /// </returns>
        Task<bool> Handle(IMessage<T> message, CancellationToken cancellationToken);
    }
}
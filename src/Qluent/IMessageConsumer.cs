namespace Qluent
{
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Interface representing an implementation of a MessageConsumer which will poll a <see cref="IAzureStorageQueue{T}" /> at some polling interview
    /// </summary>
    /// <typeparam name="T">The message payload type</typeparam>
    public interface IMessageConsumer<T>
    {
        /// <summary>
        /// Starts this MessageConsumer instance 
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A Task representing the asynchronous operation</returns>
        Task Start(CancellationToken cancellationToken);
    }
}
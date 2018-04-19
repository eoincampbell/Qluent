namespace Qluent
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    ///     An interface abstraction/wrapper around an Azure Storage Cloud Queue
    /// </summary>
    /// <typeparam name="T">The strongly type object used for interactions with the underlying <see cref="Microsoft.WindowsAzure.Storage.Queue.CloudQueue" /></typeparam>
    public interface IAzureStorageQueue<T>
    {
        /// <summary>
        ///     Initiates an asynchronous operation to push an object of type <typeparamref name="T"/> to the queue
        /// </summary>
        /// <param name="t">The object to be pushed</param>
        /// <returns>
        ///     A <see cref="Task"/> object that represents the asynchronous operation.
        /// </returns>
        Task PushAsync(T t);

        /// <summary>
        ///     Initiates an asynchronous operation to push an object of type <typeparamref name="T"/> to the queue
        /// </summary>
        /// <param name="t">The object to be pushed</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        ///     A <see cref="Task"/> object that represents the asynchronous operation.
        /// </returns>
        Task PushAsync(T t, CancellationToken cancellationToken);

        /// <summary>
        ///     Initiates an asynchronous operation to push an <see cref="IEnumerable{T}"/> of objects to the queue
        /// </summary>
        /// <param name="t">The t.</param>
        /// <returns>
        ///     A <see cref="Task"/> object that represents the asynchronous operation.
        /// </returns>
        Task PushAsync(IEnumerable<T> t);

        /// <summary>
        ///     Initiates an asynchronous operation to push an <see cref="IEnumerable{T}"/> of objects to the queue
        /// </summary>
        /// <param name="t">The t.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        ///     A <see cref="Task"/> object that represents the asynchronous operation.
        /// </returns>
        Task PushAsync(IEnumerable<T> t, CancellationToken cancellationToken);

        /// <summary>
        ///     Initiates an asynchronous operation to peek an object of type <typeparamref name="T"/> from the queue.
        /// </summary>
        /// <returns>
        ///     A <see cref="Task{T}"/> object that represents the asynchronous operation.
        ///     The task result contains the deserialized <typeparamref name="T"/>
        /// </returns>
        Task<T> PeekAsync();

        /// <summary>
        ///     Initiates an asynchronous operation to peek an object of type <typeparamref name="T"/> from the queue.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        ///     A <see cref="Task{T}"/> object that represents the asynchronous operation.
        ///     The task result contains the deserialized <typeparamref name="T"/>
        /// </returns>
        Task<T> PeekAsync(CancellationToken cancellationToken);

        /// <summary>
        ///     Initiates an asynchronous operation to peek an <see cref="IEnumerable{T}"/> of objects from the queue
        /// </summary>
        /// <param name="messageCount">The message count.</param>
        /// <returns>
        ///     A <see cref="Task{IEnumerable{T}}"/> that represents the asynchronous operation.
        ///     The task result contains an <see cref="IEnumerable{T}"/> of deserialized objects.
        /// </returns>
        Task<IEnumerable<T>> PeekAsync(int messageCount);

        /// <summary>
        ///     Initiates an asynchronous operation to peek an <see cref="IEnumerable{T}"/> of objects from the queue
        /// </summary>
        /// <param name="messageCount">The message count.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        ///     A <see cref="Task{IEnumerable{T}}"/> object that represents the asynchronous operation.
        ///     The task result contains an <see cref="IEnumerable{T}"/> of deserialized objects.
        /// </returns>
        Task<IEnumerable<T>> PeekAsync(int messageCount, CancellationToken cancellationToken);

        /// <summary>
        ///     Initiates an asynchronous operation to pop an object of type <typeparamref name="T"/> from the queue.
        /// </summary>
        /// <returns>
        ///     A <see cref="Task{T}"/> object that represents the asynchronous operation.
        ///     The task result contains the deserialized <typeparamref name="T"/>
        /// </returns>
        Task<T> PopAsync();

        /// <summary>
        ///     Initiates an asynchronous operation to pop an object of type <typeparamref name="T"/> from the queue.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        ///     A <see cref="Task{T}"/> object that represents the asynchronous operation.
        ///     The task result contains the deserialized <typeparamref name="T"/>
        /// </returns>
        Task<T> PopAsync(CancellationToken cancellationToken);

        /// <summary>
        ///     Initiates an asynchronous operation to pop an <see cref="IEnumerable{T}"/> of objects from the queue
        /// </summary>
        /// <param name="messageCount">The message count.</param>
        /// <returns>
        ///     A <see cref="Task{IEnumerable{T}}"/> object that represents the asynchronous operation.
        ///     The task result contains an <see cref="IEnumerable{T}"/> of deserialized objects.
        /// </returns>
        Task<IEnumerable<T>> PopAsync(int messageCount);

        /// <summary>
        ///     Initiates an asynchronous operation to pop an <see cref="IEnumerable{T}"/> of objects from the queue
        /// </summary>
        /// <param name="messageCount">The message count.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        ///     A <see cref="Task{IEnumerable{T}}"/> object that represents the asynchronous operation.
        ///     The task result contains an <see cref="IEnumerable{T}"/> of deserialized objects.
        /// </returns>
        Task<IEnumerable<T>> PopAsync(int messageCount, CancellationToken cancellationToken);

        /// <summary>
        ///     Initiates an asynchronous operation to get an object of type <typeparamref name="T"/> from the queue without deleting it
        /// </summary>
        /// <returns>
        ///     A <see cref="Task{IMessage{T}}"/> object that represents the asynchronous operation.
        ///     The task result contains the deserialized <typeparamref name="T"/> contained within an <see cref="IMessage{T}"/> wrapper
        /// </returns>
        Task<IMessage<T>> GetAsync();

        /// <summary>
        ///     Initiates an asynchronous operation to get an object of type <typeparamref name="T"/> from the queue without deleting it
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        ///     A <see cref="Task{IMessage{T}}"/> object that represents the asynchronous operation.
        ///     The task result contains the deserialized <typeparamref name="T"/> contained within an <see cref="IMessage{T}"/> wrapper
        /// </returns>
        Task<IMessage<T>> GetAsync(CancellationToken cancellationToken);

        /// <summary>
        ///     Initiates an asynchronous operation to get an <see cref="IEnumerable{T}"/> of objects from the queue without deleting them
        /// </summary>
        /// <param name="messageCount">The message count.</param>
        /// <returns>
        ///     A <see cref="Task{IEnumerable{IMessage{T}}}"/> object that represents the asynchronous operation.
        ///     The task result contains an <see cref="IEnumerable{IMessage{T}}"/> of deserialized <typeparamref name="T"/> 
        ///     contained within an <see cref="IMessage{T}"/> wrapper
        /// </returns>
        Task<IEnumerable<IMessage<T>>> GetAsync(int messageCount);

        /// <summary>
        ///     Initiates an asynchronous operation to get an <see cref="IEnumerable{T}"/> of objects from the queue without deleting them
        /// </summary>
        /// <param name="messageCount">The message count.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        ///     A <see cref="Task{IEnumerable{IMessage{T}}}"/> object that represents the asynchronous operation.
        ///     The task result contains an <see cref="IEnumerable{IMessage{T}}"/> of deserialized <typeparamref name="T"/> 
        ///     contained within an <see cref="IMessage{T}"/> wrapper
        /// </returns>
        Task<IEnumerable<IMessage<T>>> GetAsync(int messageCount, CancellationToken cancellationToken);

        /// <summary>
        ///     Initiates an asynchronous operation to delete an <see cref="IMessage{T}"/> from the queue using it's Id & PopReceipt
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>
        ///     A <see cref="Task"/> object that represents the asynchronous operation.
        /// </returns>
        Task DeleteAsync(IMessage<T> message);

        /// <summary>
        ///     Initiates an asynchronous operation to delete an <see cref="IMessage{T}"/> from the queue using it's Id & PopReceipt
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        ///     A <see cref="Task"/> object that represents the asynchronous operation.
        /// </returns>
        Task DeleteAsync(IMessage<T> message, CancellationToken cancellationToken);

        /// <summary>
        ///     Initiates an asynchronous operation to purge the queue of all messages
        /// </summary>
        /// <returns>
        ///     A <see cref="Task"/> object that represents the asynchronous operation.
        /// </returns>
        Task PurgeAsync();

        /// <summary>
        ///     Initiates an asynchronous operation to purge the queue of all messages
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        ///     A <see cref="Task"/> object that represents the asynchronous operation.
        /// </returns>
        Task PurgeAsync(CancellationToken cancellationToken);

        /// <summary>
        ///     Initiates an asynchronous operation to return an approximate count of the number of messages in the queue
        /// </summary>
        /// <returns>
        ///     The approximate number of messages on the queue
        /// </returns>
        Task<int?> CountAsync();

        /// <summary>
        ///     Initiates an asynchronous operation to return an approximate count of the number of messages in the queue
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        ///     The approximate number of messages on the queue
        /// </returns>
        Task<int?> CountAsync(CancellationToken cancellationToken);
    }
}

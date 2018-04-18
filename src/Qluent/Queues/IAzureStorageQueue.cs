namespace Qluent.Queues
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IAzureStorageQueue<T>
    {
        Task PushAsync(T t);
        Task PushAsync(T t, CancellationToken token);
        Task PushAsync(IEnumerable<T> t);
        Task PushAsync(IEnumerable<T> t, CancellationToken token);

        Task<T> PeekAsync();
        Task<T> PeekAsync(CancellationToken token);
        Task<IEnumerable<T>> PeekAsync(int messageCount);
        Task<IEnumerable<T>> PeekAsync(int messageCount, CancellationToken token);

        Task<T> PopAsync();
        Task<T> PopAsync(CancellationToken token);
        Task<IEnumerable<T>> PopAsync(int messageCount);
        Task<IEnumerable<T>> PopAsync(int messageCount, CancellationToken token);

        Task<Message<T>> GetAsync();
        Task<Message<T>> GetAsync(CancellationToken token);
        Task<IEnumerable<Message<T>>> GetAsync(int messageCount);
        Task<IEnumerable<Message<T>>> GetAsync(int messageCount, CancellationToken token);

        Task DeleteAsync(Message<T> message);
        Task DeleteAsync(Message<T> message, CancellationToken token);
        


        Task PurgeAsync();
        Task PurgeAsync(CancellationToken token);
        Task<int?> CountAsync();
        Task<int?> CountAsync(CancellationToken token);

    }
}
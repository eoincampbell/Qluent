namespace Qluent
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
        Task<IMessage<T>> GetAsync();
        Task<IMessage<T>> GetAsync(CancellationToken token);
        Task<IEnumerable<IMessage<T>>> GetAsync(int messageCount);
        Task<IEnumerable<IMessage<T>>> GetAsync(int messageCount, CancellationToken token);
        Task DeleteAsync(IMessage<T> message);
        Task DeleteAsync(IMessage<T> message, CancellationToken token);
        Task PurgeAsync();
        Task PurgeAsync(CancellationToken token);
        Task<int?> CountAsync();
        Task<int?> CountAsync(CancellationToken token);
    }
}
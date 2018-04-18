using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Qluent.Queues
{
    public interface IAzureStorageQueue<T>
    {
        Task PushAsync(T message);
        Task PushAsync(T message, CancellationToken token);
        Task PushAsync(IEnumerable<T> messages);
        Task PushAsync(IEnumerable<T> messages, CancellationToken token);
        Task<T> PeekAsync();
        Task<T> PeekAsync(CancellationToken token);
        Task<IEnumerable<T>> PeekAsync(int messageCount);
        Task<IEnumerable<T>> PeekAsync(int messageCount, CancellationToken token);
        Task<T> PopAsync();
        Task<T> PopAsync(CancellationToken token);
        Task<IEnumerable<T>> PopAsync(int messageCount);
        Task<IEnumerable<T>> PopAsync(int messageCount, CancellationToken token);
        Task PurgeAsync();
        Task PurgeAsync(CancellationToken token);
        Task<int?> CountAsync();
        Task<int?> CountAsync(CancellationToken token);

    }
}
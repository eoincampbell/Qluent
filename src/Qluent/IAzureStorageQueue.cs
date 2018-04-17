using System.Collections.Generic;
using System.Threading.Tasks;

namespace Qluent
{
    public interface IAzureStorageQueue<T>
    {
        Task PushAsync(T message);
        Task PushAsync(IEnumerable<T> messages);
        Task<T> PeekAsync();
        Task<IEnumerable<T>> PeekAsync(int messageCount);
        Task<T> PopAsync();
        Task<IEnumerable<T>> PopAsync(int messageCount);
        Task PurgeAsync();
        Task<int?> CountAsync();
        
    }
}
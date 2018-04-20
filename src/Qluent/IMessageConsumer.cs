using System.Threading;

namespace Qluent
{
    using System.Threading.Tasks;

    public interface IMessageConsumer<T>
    {
        Task Start(CancellationToken cancellationToken);
    }
}
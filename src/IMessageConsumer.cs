public interface IMessageConsumer<T>
{
    Task Start(CancellationToken cancellationToken);
}
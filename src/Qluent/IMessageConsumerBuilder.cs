using Qluent.Consumers.Handlers;
using Qluent.Consumers.Policies;

namespace Qluent
{
    public interface IMessageConsumerBuilder<T>
    {
        IMessageConsumerBuilder<T> UsingQueue(IAzureStorageQueue<T> queue);
        IMessageConsumerBuilder<T> ThatHandlesMessagesUsing(IMessageHandler<T> messageHandler);
        IMessageConsumerBuilder<T> AndHandlesFailedMessagesUsing(IMessageHandler<T> failedMessagehandler);
        IMessageConsumerBuilder<T> AndHandlesExceptionsUsing(IMessageExceptionHandler<T> messageExceptionHandler);
        IMessageConsumerBuilder<T> WithAQueuePolingPolicyOf(IMessageConsumerQueuePolingPolicy policy);

        IMessageConsumer<T> Build();

    }
}
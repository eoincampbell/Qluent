namespace Qluent
{
    using System;
    using Consumers.Handlers;
    using Consumers.Policies;

    public interface IMessageConsumerBuilder<T>
    {
        IMessageConsumerBuilder<T> UsingQueue(IAzureStorageQueue<T> queue);
        IMessageConsumerBuilder<T> ThatHandlesMessagesUsing(IMessageHandler<T> messageHandler);
        IMessageConsumerBuilder<T> AndHandlesFailedMessagesUsing(IMessageHandler<T> failedMessagehandler);
        IMessageConsumerBuilder<T> AndHandlesExceptionsUsing(IMessageExceptionHandler<T> messageExceptionHandler);
        IMessageConsumerBuilder<T> ThatHandlesMessagesUsing(Func<IMessage<T>, bool> messageHandler);
        IMessageConsumerBuilder<T> AndHandlesFailedMessagesUsing(Func<IMessage<T>, bool> failedMessagehandler);
        IMessageConsumerBuilder<T> AndHandlesExceptionsUsing(Func<IMessage<T>, Exception, bool> messageExceptionHandler);
        IMessageConsumerBuilder<T> WithAQueuePolingPolicyOf(IMessageConsumerQueuePolingPolicy policy);

        IMessageConsumer<T> Build();

    }
}
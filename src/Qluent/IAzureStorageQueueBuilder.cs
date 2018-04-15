using System;

namespace Qluent
{
    public interface IAzureStorageQueueBuilder<T>
    {
        IAzureStorageQueue<T> Build();

        IAzureStorageQueueBuilder<T> ConnectedToAccount(string connectionString);

        IAzureStorageQueueBuilder<T> UsingStorageQueue(string queueName);

        IAzureStorageQueueBuilder<T> WithACustomSerializer(Func<T, string> serlializer);

        IAzureStorageQueueBuilder<T> WithACustomDeserializer(Func<string, T> deserializer);

        IAzureStorageQueueBuilder<T> ThatIsTransactionScopeAware();

        IAzureStorageQueueBuilder<T> ThatSendsPoisonMessagesToThisQueue(string poisonQueueName);
        IAzureStorageQueueBuilder<T> AndSwallowsExceptionsOnPoisonMessages();
    }
}
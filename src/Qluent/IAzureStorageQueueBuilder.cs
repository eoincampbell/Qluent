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

        IAzureStorageQueueBuilder<T> ThatSendsPoisonMessagesTo(string poisonQueueName, int afterAttempts = 3);

        IAzureStorageQueueBuilder<T> AndSwallowsExceptionsOnPoisonMessages();

        IAzureStorageQueueBuilder<T> WhereMessageVisibilityTimesOutAfter(int milliseconds = 30000);
    }
}
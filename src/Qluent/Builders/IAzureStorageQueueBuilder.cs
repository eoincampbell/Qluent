using Qluent.Queues;
using Qluent.Serialization;

namespace Qluent.Builders
{
    public interface IAzureStorageQueueBuilder<T>
    {
        IAzureStorageQueue<T> Build();

        IAzureStorageQueueBuilder<T> ConnectedToAccount(string connectionString);

        IAzureStorageQueueBuilder<T> UsingStorageQueue(string queueName);

        IAzureStorageQueueBuilder<T> WithACustomStringSerializer(IStringMessageSerializer<T> customSerlializer);
        
        IAzureStorageQueueBuilder<T> WithACustomBinarySerializer(IBinaryMessageSerializer<T> customSerlializer);      
        
        IAzureStorageQueueBuilder<T> ThatIsTransactionScopeAware();

        IAzureStorageQueueBuilder<T> ThatSendsPoisonMessagesTo(string poisonQueueName, int afterAttempts = 3);

        IAzureStorageQueueBuilder<T> AndSwallowExceptionsOnPoisonMessages();

        IAzureStorageQueueBuilder<T> WhereMessageVisibilityTimesOutAfter(int milliseconds = 30000);
    }
}
using Qluent.Policies;
using Qluent.Queues;
using Qluent.Serialization;
using System;
using System.Threading.Tasks;

namespace Qluent.Builders
{
    public interface IAzureStorageQueueBuilder<T>
    {
        IAzureStorageQueue<T> Build();
        Task<IAzureStorageQueue<T>> BuildAsync();
        IAzureStorageQueueBuilder<T> ConnectedToAccount(string connectionString);
        IAzureStorageQueueBuilder<T> UsingStorageQueue(string storageQueueName);
        IAzureStorageQueueBuilder<T> WithACustomSerializer(IStringMessageSerializer<T> customSerlializer);
        IAzureStorageQueueBuilder<T> WithACustomSerializer(IBinaryMessageSerializer<T> customSerlializer);
        IAzureStorageQueueBuilder<T> ThatIsTransactionScopeAware();
        IAzureStorageQueueBuilder<T> ThatConsidersMessagesPoisonAfter(int dequeueAttempts);
        IAzureStorageQueueBuilder<T> AndSendsPoisonMessagesTo(string poisonQueueName);
        IAzureStorageQueueBuilder<T> AndHandlesExceptionsOnPoisonMessagesBy(PoisonMessageBehavior behavior);
        IAzureStorageQueueBuilder<T> ThatDelaysMessageVisibilityAfterEnqueuingFor(TimeSpan timespan);
        IAzureStorageQueueBuilder<T> ThatKeepsMessagesInvisibleAfterDequeuingFor(TimeSpan timespan);
        IAzureStorageQueueBuilder<T> ThatSetsAMessageTTLOf(TimeSpan timespan);
    }
}
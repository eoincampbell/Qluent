using System;
using System.Threading.Tasks;
using Qluent.Policies;
using Qluent.Queues;
using Qluent.Serialization;

namespace Qluent.Builders
{
    internal class AzureStorageQueueBuilder<T> : IAzureStorageQueueBuilder<T>
    {
        private readonly IAzureStorageQueueSettings _settings;
        private readonly IMessageTimeoutPolicy _messageTimeoutPolicy;
        private readonly IPoisonMessageBehaviorPolicy _poisonMessageBehaviorPolicy;

        private IStringMessageSerializer<T> _customStringSerializer;
        private IBinaryMessageSerializer<T> _customBinarySerializer;

        private bool _transactionScopeAware;

        public AzureStorageQueueBuilder()
        {
            _settings = new AzureStorageQueueSettings();
            _messageTimeoutPolicy = new MessageTimeoutPolicy();
            _poisonMessageBehaviorPolicy = new PoisonMessageBehaviorPolicy();
            _customStringSerializer = null;
            _customBinarySerializer = null;
            _transactionScopeAware = false;
        }

        public IAzureStorageQueue<T> Build()
        {
            var queue = _transactionScopeAware
                ? TransactionalAzureStorageQueue<T>.CreateAsync(_settings,
                            _messageTimeoutPolicy,
                            _poisonMessageBehaviorPolicy,
                            _customStringSerializer,
                            _customBinarySerializer).Result
                : SimpleAzureStorageQueue<T>.CreateAsync(_settings,
                            _messageTimeoutPolicy,
                            _poisonMessageBehaviorPolicy,
                            _customStringSerializer,
                            _customBinarySerializer).Result;

            return queue;
        }

        public async Task<IAzureStorageQueue<T>> BuildAsync()
        {

            var queue = _transactionScopeAware
                ? await TransactionalAzureStorageQueue<T>.CreateAsync(_settings,
                            _messageTimeoutPolicy,
                            _poisonMessageBehaviorPolicy,
                            _customStringSerializer,
                            _customBinarySerializer)
                            .ConfigureAwait(false)
                : await SimpleAzureStorageQueue<T>.CreateAsync(_settings,
                            _messageTimeoutPolicy,
                            _poisonMessageBehaviorPolicy,
                            _customStringSerializer,
                            _customBinarySerializer)
                            .ConfigureAwait(false);

            return queue;
        }

        public IAzureStorageQueueBuilder<T> ConnectedToAccount(string connectionString)
        {
            _settings.ConnectionString = connectionString;
            return this;
        }

        public IAzureStorageQueueBuilder<T> UsingStorageQueue(string storageQueueName)
        {
            _settings.StorageQueueName = storageQueueName;
            return this;
        }

        public IAzureStorageQueueBuilder<T> WithACustomSerializer(IStringMessageSerializer<T> customSerlializer)
        {
            _customStringSerializer = customSerlializer;
            return this;
        }

        public IAzureStorageQueueBuilder<T> WithACustomSerializer(IBinaryMessageSerializer<T> customSerlializer)
        {
            _customBinarySerializer = customSerlializer;
            return this;
        }

        public IAzureStorageQueueBuilder<T> ThatIsTransactionScopeAware()
        {
            _transactionScopeAware = true;
            return this;
        }

        public IAzureStorageQueueBuilder<T> ThatConsidersMessagesPoisonAfter(int dequeueAttempts)
        {
            _poisonMessageBehaviorPolicy.PoisonMessageDequeueAttemptThreshold = dequeueAttempts;
            return this;
        }

        public IAzureStorageQueueBuilder<T> AndSendsPoisonMessagesTo(string poisonQueueName)
        {
            _poisonMessageBehaviorPolicy.PoisonMessageStorageQueueName = poisonQueueName;
            return this;
        }

        public IAzureStorageQueueBuilder<T> AndHandlesExceptionsOnPoisonMessagesBy(PoisonMessageBehavior behavior)
        {
            _poisonMessageBehaviorPolicy.PoisonMessageBehavior = behavior;
            return this;
        }

        public IAzureStorageQueueBuilder<T> ThatDelaysMessageVisibilityAfterEnqueuingFor(TimeSpan timespan)
        {
            _messageTimeoutPolicy.InitialVisibilityDelay = timespan;
            return this;
        }

        public IAzureStorageQueueBuilder<T> ThatKeepsMessagesInvisibleAfterDequeuingFor(TimeSpan timespan)
        {
            _messageTimeoutPolicy.VisibilityTimeout = timespan;
            return this;
        }

        public IAzureStorageQueueBuilder<T> ThatSetsAMessageTTLOf(TimeSpan timespan)
        {
            _messageTimeoutPolicy.TimeToLive = timespan;
            return this;

        }
    }
}
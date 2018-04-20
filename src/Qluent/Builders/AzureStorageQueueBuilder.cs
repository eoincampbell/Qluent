namespace Qluent.Builders
{
    using System;
    using System.Threading.Tasks;
    using Queues.Policies;
    using Queues.Policies.PoisonMessageBehavior;
    using Queues;
    using Serialization;

    internal class AzureStorageQueueBuilder<T> : IAzureStorageQueueBuilder<T>
    {
        private readonly IAzureStorageQueueSettings _settings;
        private readonly IMessageTimeoutPolicy _messageTimeoutPolicy;
        private readonly IPoisonMessageBehaviorPolicy _poisonMessageBehaviorPolicy;

        private IStringMessageSerializer<T> _customStringSerializer;
        private IBinaryMessageSerializer<T> _customBinarySerializer;

        public AzureStorageQueueBuilder()
        {
            _settings = new AzureStorageQueueSettings();
            _messageTimeoutPolicy = new MessageTimeoutPolicy();
            _poisonMessageBehaviorPolicy = new PoisonMessageBehaviorPolicy();
            _customStringSerializer = null;
            _customBinarySerializer = null;
        }

        public IAzureStorageQueue<T> Build()
        {
            return AzureStorageQueue<T>.CreateAsync(_settings,
                            _messageTimeoutPolicy,
                            _poisonMessageBehaviorPolicy,
                            _customStringSerializer,
                            _customBinarySerializer).Result;
        }

        public async Task<IAzureStorageQueue<T>> BuildAsync()
        {
            return await AzureStorageQueue<T>.CreateAsync(_settings,
                            _messageTimeoutPolicy,
                            _poisonMessageBehaviorPolicy,
                            _customStringSerializer,
                            _customBinarySerializer)
                            .ConfigureAwait(false);
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

        public IAzureStorageQueueBuilder<T> AndHandlesExceptionsOnPoisonMessages(By behavior)
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

        public IAzureStorageQueueBuilder<T> ThatSetsAMessageTtlOf(TimeSpan timespan)
        {
            _messageTimeoutPolicy.TimeToLive = timespan;
            return this;

        }
    }
}
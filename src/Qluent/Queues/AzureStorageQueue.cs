namespace Qluent.Queues
{
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Queue;
    using Qluent.Policies;
    using Qluent.Policies.PoisonMessageBehavior;
    using Qluent.Serialization;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    internal class AzureStorageQueue<T> : IAzureStorageQueue<T>
    {
        private CloudQueue _cloudQueue;
        private CloudQueue _poisonQueue;

        private readonly IAzureStorageQueueSettings _settings;
        private readonly IMessageTimeoutPolicy _messageTimeoutPolicy;
        private readonly IMessageSerializer<T, string> _defaultSerializer = new DefaultMessageSerializer<T>();
        private readonly IStringMessageSerializer<T> _customStringSerializer;
        private readonly IBinaryMessageSerializer<T> _customBinarySerializer;
        private readonly IPoisonMessageBehaviorPolicy _poisonMessageBehaviorPolicy;

        protected AzureStorageQueue(
            IAzureStorageQueueSettings settings,
            IMessageTimeoutPolicy messageTimeoutPolicy,
            IPoisonMessageBehaviorPolicy poisonMessageBehaviorPolicy = null,
            IStringMessageSerializer<T> customStringSerializer = null,
            IBinaryMessageSerializer<T> customBinarySerializer = null)
        {
            _settings = settings;
            _messageTimeoutPolicy = messageTimeoutPolicy;
            _poisonMessageBehaviorPolicy = poisonMessageBehaviorPolicy;
            _customStringSerializer = customStringSerializer;
            _customBinarySerializer = customBinarySerializer;
        }

        public static async Task<AzureStorageQueue<T>> CreateAsync(
            IAzureStorageQueueSettings settings, 
            IMessageTimeoutPolicy messageTimeoutPolicy,
            IPoisonMessageBehaviorPolicy poisonMessageBehaviorPolicy = null,
            IStringMessageSerializer<T> customStringSerializer = null,
            IBinaryMessageSerializer<T> customBinarySerializer = null)
        {

            var queue = new AzureStorageQueue<T>(settings,
                            messageTimeoutPolicy,
                            poisonMessageBehaviorPolicy,
                            customStringSerializer,
                            customBinarySerializer);

            await queue
                .InstantiateQueues()
                .ConfigureAwait(false);

            return queue;
        }

        protected async Task InstantiateQueues()
        {
            var cloudStorageAccount = CloudStorageAccount.Parse(_settings.ConnectionString);
            var cloudQueueClient = cloudStorageAccount.CreateCloudQueueClient();

            _cloudQueue = cloudQueueClient.GetQueueReference(_settings.StorageQueueName);

            await _cloudQueue
                .CreateIfNotExistsAsync()
                .ConfigureAwait(false);
            
            if(!string.IsNullOrWhiteSpace(_poisonMessageBehaviorPolicy?.PoisonMessageStorageQueueName))
            {
                _poisonQueue = cloudQueueClient.GetQueueReference(_poisonMessageBehaviorPolicy.PoisonMessageStorageQueueName);

                await _poisonQueue
                    .CreateIfNotExistsAsync()
                    .ConfigureAwait(false);
            }
        }

        public virtual async Task PushAsync(T message)
        {
            await PushAsync(message, new CancellationToken())
                .ConfigureAwait(false);
        }

        public virtual async Task PushAsync(T message, CancellationToken token)
        {
            await Enqueue(message, token)
                .ConfigureAwait(false);
        }

        public virtual async Task PushAsync(IEnumerable<T> messages)
        {
            await PushAsync(messages, new CancellationToken())
                .ConfigureAwait(false);
        }

        public virtual async Task PushAsync(IEnumerable<T> messages, CancellationToken token)
        {
            foreach (var message in messages)
            {
                await Enqueue(message, token)
                    .ConfigureAwait(false);
            }
        }
        public async Task<T> PeekAsync()
        {
            return await PeekAsync(new CancellationToken())
                .ConfigureAwait(false);
        }

        public async Task<T> PeekAsync(CancellationToken token)
        {
            var qMsg = await _cloudQueue
                .PeekMessageAsync(null, null, token)
                .ConfigureAwait(false);

            if (qMsg == null)
            {
                return default(T);
            }

            var obj = await FromCloudQueueMessage(qMsg, token)
                .ConfigureAwait(false);

            return obj;
        }

        public async Task<IEnumerable<T>> PeekAsync(int messageCount)
        {
            return await PeekAsync(messageCount, new CancellationToken())
                .ConfigureAwait(false);
        }

        public async Task<IEnumerable<T>> PeekAsync(int messageCount, CancellationToken token)
        {
            var qMsgs = await _cloudQueue
                .PeekMessagesAsync(messageCount)
                .ConfigureAwait(false);

            var objs = new List<T>();

            foreach (var qMsg in qMsgs)
            {
                var obj = await FromCloudQueueMessage(qMsg, token)
                    .ConfigureAwait(false);

                if (obj != null)
                {
                    objs.Add(obj);
                }
            }

            return objs;
        }

        public async Task<T> PopAsync()
        {
            return await PopAsync(new CancellationToken())
                .ConfigureAwait(false);
        }

        public async Task<T> PopAsync(CancellationToken token)
        {
            var qMsg = await _cloudQueue
                .GetMessageAsync(_messageTimeoutPolicy.VisibilityTimeout, null, null, token)
                .ConfigureAwait(false);

            if (qMsg == null)
            {
                return default(T);
            }

            var obj = await FromCloudQueueMessage(qMsg, token)
                .ConfigureAwait(false);

            if (obj != null)
            {
                await _cloudQueue
                    .DeleteMessageAsync(qMsg.Id, qMsg.PopReceipt, null, null, token)
                    .ConfigureAwait(false);
            }

            return obj;
        }

        public async Task<IEnumerable<T>> PopAsync(int messageCount)
        {
            return await PopAsync(messageCount, new CancellationToken())
                .ConfigureAwait(false);
        }

        public async Task<IEnumerable<T>> PopAsync(int messageCount, CancellationToken token)
        {
            var qMsgs = await _cloudQueue
                .GetMessagesAsync(messageCount, _messageTimeoutPolicy.VisibilityTimeout, null, null, token)
                .ConfigureAwait(false);


            var objs = new List<T>();
            var qMsgsToDelete = new List<CloudQueueMessage>();

            foreach (var qMsg in qMsgs)
            {
                var obj = await FromCloudQueueMessage(qMsg, token)
                    .ConfigureAwait(false);

                if (obj != null)
                {
                    objs.Add(obj);
                    qMsgsToDelete.Add(qMsg);
                }
            }

            foreach (var qMsg in qMsgsToDelete)
            {
                await _cloudQueue
                    .DeleteMessageAsync(qMsg.Id, qMsg.PopReceipt, null, null, token)
                    .ConfigureAwait(false);
            }
            return objs;
        }

        public async Task PurgeAsync()
        {
            await PurgeAsync(new CancellationToken())
                .ConfigureAwait(false);
        }

        public async Task PurgeAsync(CancellationToken token)
        {
            await _cloudQueue
                .ClearAsync(null, null, token)
                .ConfigureAwait(false);
        }

        public async Task<int?> CountAsync()
        {
            return await CountAsync(new CancellationToken())
                .ConfigureAwait(false);
        }

        public async Task<int?> CountAsync(CancellationToken token)
        {

            await _cloudQueue
            .FetchAttributesAsync(null, null, token)
            .ConfigureAwait(false);

            return _cloudQueue.ApproximateMessageCount;
        }

        public async Task<Message<T>> GetAsync()
        {
            return await GetAsync(new CancellationToken())
                .ConfigureAwait(false);
        }

        public async Task<Message<T>> GetAsync(CancellationToken token)
        {
            var qMsg = await _cloudQueue
                .GetMessageAsync(_messageTimeoutPolicy.VisibilityTimeout, null, null, token)
                .ConfigureAwait(false);

            if (qMsg == null)
            {
                return null;
            }

            var obj = await FromCloudQueueMessage(qMsg, token)
                .ConfigureAwait(false);

            if(obj == null)
            {
                return null;
            }

            return new Message<T>(qMsg.Id, qMsg.PopReceipt, obj);
        }

        public async Task<IEnumerable<Message<T>>> GetAsync(int messageCount)
        {
            return await GetAsync(messageCount, new CancellationToken())
                .ConfigureAwait(false);
        }

        public async Task<IEnumerable<Message<T>>> GetAsync(int messageCount, CancellationToken token)
        {
            var qMsgs = await _cloudQueue
                .GetMessagesAsync(messageCount, _messageTimeoutPolicy.VisibilityTimeout, null, null, token)
                .ConfigureAwait(false);

            var messages = new List<Message<T>>();

            foreach (var qMsg in qMsgs)
            {
                var obj = await FromCloudQueueMessage(qMsg, token)
                    .ConfigureAwait(false);

                if (obj != null)
                {
                    var message = new Message<T>(qMsg.Id, qMsg.PopReceipt, obj);
                    messages.Add(message);
                }
            }

            return messages;
        }

        public async Task DeleteAsync(Message<T> message)
        {
            await DeleteAsync(message, new CancellationToken())
                .ConfigureAwait(false);
        }

        public async Task DeleteAsync(Message<T> message, CancellationToken token)
        {
            await _cloudQueue
                .DeleteMessageAsync(message.MessageId, message.PopReceipt, null, null, token)
                .ConfigureAwait(false);
        }

        #region Internal Functionality 

        protected async Task Enqueue(T entity, CancellationToken token)
        {
            var qMsg = ToCloudQueueMessage(entity);

            await _cloudQueue
                .AddMessageAsync(qMsg, _messageTimeoutPolicy.TimeToLive, _messageTimeoutPolicy.InitialVisibilityDelay, null, null, token)
                .ConfigureAwait(false);
        }
        
        private CloudQueueMessage ToBinaryCloudQueueMessage(T entity)
        {
            var serializedMessage = _customBinarySerializer.Serialize(entity);
            return ToBinaryCloudQueueMessage(serializedMessage);
        }

        private CloudQueueMessage ToBinaryCloudQueueMessage(byte [] bytes)
        {
            var qMsg = new CloudQueueMessage(string.Empty);
            qMsg.SetMessageContent(bytes);
            return qMsg;
        }

        private CloudQueueMessage ToStringCloudQueueMessage(T entity)
        {
            var serializedMessage = (_customStringSerializer ?? _defaultSerializer).Serialize(entity);
            return ToStringCloudQueueMessage(serializedMessage);
        }

        private CloudQueueMessage ToStringCloudQueueMessage(string message)
        {
            return new CloudQueueMessage(message);
        }
        private CloudQueueMessage ToCloudQueueMessage(T entity)
        {
            return _customBinarySerializer != null
                ? ToBinaryCloudQueueMessage(entity)
                : ToStringCloudQueueMessage(entity);
        }

        private CloudQueueMessage ToCloudQueueMessage(CloudQueueMessage poisonMessage)
        {
            return _customBinarySerializer != null
                ? ToBinaryCloudQueueMessage(poisonMessage.AsBytes)
                : ToStringCloudQueueMessage(poisonMessage.AsString);
        }

        private async Task<T> FromCloudQueueMessage(CloudQueueMessage qMsg, CancellationToken token)
        {
            try
            {
                return _customBinarySerializer != null
                    ? _customBinarySerializer.Deserialize(qMsg.AsBytes)
                    : (_customStringSerializer ?? _defaultSerializer).Deserialize(qMsg.AsString);
            }
            catch
            {
                try
                {
                    if (_poisonQueue != null && 
                        _poisonMessageBehaviorPolicy.PoisonMessageDequeueAttemptThreshold <= qMsg.DequeueCount)
                    {
                        var poisonMessage = ToCloudQueueMessage(qMsg);

                        await _poisonQueue
                             .AddMessageAsync(qMsg, null, null, null, null, token)
                            .ConfigureAwait(false);
                    }
                    if (_poisonMessageBehaviorPolicy.PoisonMessageDequeueAttemptThreshold <= qMsg.DequeueCount)
                    {
                        await _cloudQueue
                            .DeleteMessageAsync(qMsg.Id, qMsg.PopReceipt, null, null, token)
                            .ConfigureAwait(false);
                    }
                }
                catch
                {
                    //Swallow exceptions relating to Poison Message reenquement for now.
                }

                if (_poisonMessageBehaviorPolicy.PoisonMessageBehavior == By.SwallowingExceptions)
                {
                    return default(T);
                }

                throw;
            }

        }

        #endregion
    }
}
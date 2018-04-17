using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Qluent.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Qluent.Queues
{
    public enum BehaviorOnPoisonMessage
    {
        ThrowException,
        SwallowException
    }

    internal class SimpleAzureStorageQueue<T> : IAzureStorageQueue<T>
    {
        private readonly CloudQueue _cloudQueue;
        private readonly CloudQueue _poisonQueue;


        private readonly IMessageSerializer<T, string> _defaultSerializer = new DefaultMessageSerializer<T>();

        private readonly IStringMessageSerializer<T> _customStringSerializer;
        private readonly IBinaryMessageSerializer<T> _customBinarySerializer;

        private readonly BehaviorOnPoisonMessage _behaviorOnPoisonMessage;
        private readonly int _considerPoisonAfterDequeueAttempts;
        private readonly int _visibilityTimeout;


        internal SimpleAzureStorageQueue(
            string connectionString, 
            string queueName,

            BehaviorOnPoisonMessage behaviorOnPoisonMessage = BehaviorOnPoisonMessage.ThrowException,
            string poisonQueueName = null,
            int considerPoisonAfterAttemptCount = 5,

            IStringMessageSerializer<T> customStringSerializer = null,
            IBinaryMessageSerializer<T> customBinarySerializer = null,

            int visibilityTimeout = 30000)
        { 
            var cloudStorageAccount = CloudStorageAccount.Parse(connectionString);
            var cloudQueueClient = cloudStorageAccount.CreateCloudQueueClient();

            _cloudQueue = cloudQueueClient.GetQueueReference(queueName);
            _cloudQueue.CreateIfNotExistsAsync();

            if (!string.IsNullOrWhiteSpace(poisonQueueName))
            {
                _poisonQueue = cloudQueueClient.GetQueueReference(poisonQueueName);
                _poisonQueue.CreateIfNotExistsAsync();
            }

            _considerPoisonAfterDequeueAttempts = considerPoisonAfterAttemptCount;

            _customStringSerializer = customStringSerializer;
            _customBinarySerializer = customBinarySerializer;

            _behaviorOnPoisonMessage = behaviorOnPoisonMessage;

            _visibilityTimeout = visibilityTimeout;

        }

        public virtual async Task PushAsync(T message)
        {
            await Enqueue(message);
        }

        public virtual async Task PushAsync(IEnumerable<T> messages)
        {
            foreach (var message in messages)
            {
                await Enqueue(message);
            }
        }

        public async Task<T> PeekAsync()
        {
            var qMsg = await _cloudQueue.PeekMessageAsync();

            var deserializedMessage = await FromCloudQueueMessage(qMsg);

            return deserializedMessage;
        }

        public async Task<IEnumerable<T>> PeekAsync(int messageCount)
        {
            var qMsgs = (await _cloudQueue.PeekMessagesAsync(messageCount)).ToList();

            var objects = new List<T>();

            foreach (var qMsg in qMsgs)
            {
                var deserializedMessage = await FromCloudQueueMessage(qMsg);

                if (deserializedMessage != null)
                {
                    objects.Add(deserializedMessage);
                }
            }

            return objects;
        }

        public async Task<T> PopAsync()
        {
            var qMsg = await _cloudQueue.GetMessageAsync(TimeSpan.FromMilliseconds(_visibilityTimeout), null, null);

            if (qMsg == null)
            {
                return default(T);
            }

            var deserializedMessage = await FromCloudQueueMessage(qMsg);

            if (deserializedMessage != null)
            {
                await _cloudQueue.DeleteMessageAsync(qMsg);
            }

            return deserializedMessage;
        }

        public async Task<IEnumerable<T>> PopAsync(int messageCount)
        {
            var qMsgs = (await _cloudQueue.GetMessagesAsync(messageCount, TimeSpan.FromMilliseconds(_visibilityTimeout), null, null)).ToList();

            var objects = new List<T>();
            var qMsgsToDelete = new List<CloudQueueMessage>();

            foreach (var qMsg in qMsgs)
            {
                var deserializedMessage = await FromCloudQueueMessage(qMsg);

                if (deserializedMessage != null)
                {
                    objects.Add(deserializedMessage);
                    qMsgsToDelete.Add(qMsg);
                }
            }

            foreach (var qMsg in qMsgsToDelete)
            {
                await _cloudQueue.DeleteMessageAsync(qMsg);
            }
            return objects;
        }

        public async Task PurgeAsync()
        {
            await _cloudQueue.ClearAsync();
        }

        public async Task<int?> CountAsync()
        {
            await _cloudQueue.FetchAttributesAsync();
            return _cloudQueue.ApproximateMessageCount;
        }

        #region Implements IAzureStorageQueue

        protected async Task Enqueue(T entity)
        {
            var qMsg = ToCloudQueueMessage(entity);

            await _cloudQueue.AddMessageAsync(qMsg);
        }

        private CloudQueueMessage ToCloudQueueMessage(T entity)
        {
            CloudQueueMessage qMsg = null;
            if (_customBinarySerializer != null)
            {
                byte[] serializedMessage = _customBinarySerializer.Serialize(entity);
                qMsg = new CloudQueueMessage("");
                qMsg.SetMessageContent(serializedMessage);
            }
            else
            {
                var serializedMessage = (_customStringSerializer ?? _defaultSerializer).Serialize(entity);
                qMsg = new CloudQueueMessage(serializedMessage);
            }

            return qMsg;
        }

        private CloudQueueMessage ToCloudQueueMessage(CloudQueueMessage poisonMessage)
        {
            CloudQueueMessage qMsg = null;
            if (_customBinarySerializer != null)
            {
                qMsg = new CloudQueueMessage(string.Empty);
                qMsg.SetMessageContent(poisonMessage.AsBytes);
            }
            else
            {
                qMsg = new CloudQueueMessage(poisonMessage.AsString);
            }

            return qMsg;
        }

        private async Task<T> FromCloudQueueMessage(CloudQueueMessage qMsg)
        {
            try
            {
                if (_customBinarySerializer != null)
                {
                    return _customBinarySerializer.Deserialize(qMsg.AsBytes);
                }
                else
                {
                    return (_customStringSerializer ?? _defaultSerializer).Deserialize(qMsg.AsString);
                }
            }
            catch
            {
                try
                {
                    if (_poisonQueue != null && _considerPoisonAfterDequeueAttempts <= qMsg.DequeueCount)
                    {
                        var poisonMessage = ToCloudQueueMessage(qMsg);

                        await _poisonQueue.AddMessageAsync(qMsg);
                    }
                    if (_considerPoisonAfterDequeueAttempts <= qMsg.DequeueCount)
                    {
                        await _cloudQueue.DeleteMessageAsync(qMsg);
                    }
                }
                catch
                {
                    // ignored
                }

                if (_behaviorOnPoisonMessage == BehaviorOnPoisonMessage.SwallowException)
                {
                    return default(T);
                }
                throw;
            }

        }

        #endregion
    }
}
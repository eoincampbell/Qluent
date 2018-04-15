using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
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

        private readonly Func<T, string> _defaultSerialize = (obj) => JsonConvert.SerializeObject(obj);
        private readonly Func<string, T> _defaultDeserialize = JsonConvert.DeserializeObject<T>;

        private readonly Func<T, string> _customSerialize;
        private readonly Func<string, T> _customDeserialize;

        private readonly BehaviorOnPoisonMessage _behaviorOnPoisonMessage;


        internal SimpleAzureStorageQueue(
            string connectionString, 
            string queueName,
            BehaviorOnPoisonMessage behaviorOnPoisonMessage = BehaviorOnPoisonMessage.ThrowException,
            string poisonQueueName = null,
            Func<T, string> serlializer = null,
            Func<string, T> deserializer = null)
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

            _customSerialize = serlializer;
            _customDeserialize = deserializer;

            _behaviorOnPoisonMessage = behaviorOnPoisonMessage;

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

        protected async Task Enqueue(T message)
        {
            var serializedMessage = (_customSerialize ?? _defaultSerialize)(message);

            CloudQueueMessage qMsg = new CloudQueueMessage(serializedMessage);

            await _cloudQueue.AddMessageAsync(qMsg);
        }

        public async Task<T> PeekAsync()
        {
            var qMsg = await _cloudQueue.PeekMessageAsync();

            var deserializedMessage = await GetDeserializedMessage(qMsg);

            return deserializedMessage;
        }

        private async Task<T> GetDeserializedMessage(CloudQueueMessage qMsg)
        {

            try
            {
                return (_customDeserialize ?? _defaultDeserialize)(qMsg.AsString);
            }
            catch
            {
                try
                {
                    if (_poisonQueue != null)
                    {
                        var poisonMessage = new CloudQueueMessage(qMsg.AsString);

                        await _poisonQueue.AddMessageAsync(poisonMessage);
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

        public async Task<T> PopAsync()
        {
            var qMsg = await _cloudQueue.GetMessageAsync();

            var deserializedMessage = await GetDeserializedMessage(qMsg);

            await _cloudQueue.DeleteMessageAsync(qMsg);

            return deserializedMessage;
        }

        public async Task<IEnumerable<T>> PopAsync(int messageCount)
        {
            var qMsgs = (await _cloudQueue.GetMessagesAsync(messageCount)).ToList();

            var objects = new List<T>();

            foreach (var qMsg in qMsgs)
            {
                var deserializedMessage = await GetDeserializedMessage(qMsg);

                objects.Add(deserializedMessage);
            }

            foreach (var qMsg in qMsgs)
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
    }
}
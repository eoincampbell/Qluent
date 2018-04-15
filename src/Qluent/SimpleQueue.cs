using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;

namespace Qluent
{
    
    public interface ISimpleQueue<T>
    {
        Task PushAsync(T message);
        Task PushAsync(IEnumerable<T> messages);
        Task<T> PeekAsync();
        Task<T> PopAsync();

        Task<T> PopAsync(int numMessages);

        Task PurgeAsync();

        Task<int?> CountAsync();
        
    }

    internal class SimpleQueue<T> : ISimpleQueue<T>, IEnlistmentNotification
    {
        #region IEnlistmentNotification For TransactionScope Support

        public async void Commit(Enlistment enlistment)
        {
            foreach (var message in _deferredMessages)
            {
                await Enqueue(message);
            }
            enlistment.Done();
        }

        public void InDoubt(Enlistment enlistment)
        {
            enlistment.Done();
        }

        //Don't throw an exception here. Instead call ForceRollback()
        public void Prepare(PreparingEnlistment preparingEnlistment)
        {
            try
            {
                preparingEnlistment.Prepared();
            }
            catch (Exception e)
            {
                preparingEnlistment.ForceRollback(e);
            }
        }

        public void Rollback(Enlistment enlistment)
        {
            _deferEnqueueUntilCommitted = false;
            _deferredMessages = new List<T>();
            enlistment.Done();
        }

        private void AttemptEnlistment()
        {
            if (Transaction.Current == null)
            {
                return;
            }

            Transaction.Current.EnlistVolatile(this, EnlistmentOptions.None);
            _deferredMessages = new List<T>();
            _deferEnqueueUntilCommitted = true;
        }


        #endregion

        private bool _deferEnqueueUntilCommitted;
        private List<T> _deferredMessages;

        private readonly CloudQueue _cloudQueue;

        private readonly Func<T, string> _defaultSerialize = (obj) => JsonConvert.SerializeObject(obj);
        private readonly Func<string, T> _defaultDeserialize = JsonConvert.DeserializeObject<T>;

        private readonly Func<T, string> _customSerialize;
        private readonly Func<string, T> _customDeserialize;

        internal SimpleQueue(string connectionString, string queueName, Func<T, string> serlializer = null, Func<string, T> deserializer = null)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException(nameof(connectionString));
            }

            if (string.IsNullOrWhiteSpace(queueName))
            {
                throw new ArgumentException(nameof(queueName));
            }

            _customSerialize = serlializer;
            _customDeserialize = deserializer;

            var cloudStorageAccount = CloudStorageAccount.Parse(connectionString);
            var cloudQueueClient = cloudStorageAccount.CreateCloudQueueClient();
            _cloudQueue = cloudQueueClient.GetQueueReference(queueName);

            _cloudQueue.CreateIfNotExistsAsync();

            _deferredMessages = new List<T>();
        }

        public async Task PushAsync(T message)
        {
            AttemptEnlistment();
            if (_deferEnqueueUntilCommitted)
            {
                _deferredMessages.Add(message);
            }
            else
            { 
                await Enqueue(message);
            }
        }

        public async Task PushAsync(IEnumerable<T> messages)
        {
            AttemptEnlistment();
            if (_deferEnqueueUntilCommitted)
            {
                _deferredMessages.AddRange(messages);
            }
            else
            {
                foreach (var message in messages)
                {
                    await Enqueue(message);
                }
            }
        }

        private async Task Enqueue(T message)
        {
            var serializedMessage = (_customSerialize ?? _defaultSerialize)(message);

            CloudQueueMessage qMsg = new CloudQueueMessage(serializedMessage);

            await _cloudQueue.AddMessageAsync(qMsg);
        }

        public async Task<T> PeekAsync()
        {
            var qMsg = await _cloudQueue.PeekMessageAsync();

            var deserializedMessage = (_customDeserialize ?? _defaultDeserialize)(qMsg.AsString);

            return deserializedMessage;
        }

        public async Task<T> PopAsync()
        {
            var qMsg = await _cloudQueue.GetMessageAsync();

            var deserializedMessage = (_customDeserialize ?? _defaultDeserialize)(qMsg.AsString);

            await _cloudQueue.DeleteMessageAsync(qMsg);

            return deserializedMessage;
        }
        public async Task<T> PopAsync(int messages)
        {
            var qMsg = await _cloudQueue.GetMessageAsync();

            var deserializedMessage = (_customDeserialize ?? _defaultDeserialize)(qMsg.AsString);

            await _cloudQueue.DeleteMessageAsync(qMsg);

            return deserializedMessage;
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

    public interface ISimpleQueueBuilder<T>
    {
        ISimpleQueue<T> Build();

        ISimpleQueueBuilder<T> ConnectedToAccount(string connectionString);

        ISimpleQueueBuilder<T> UsingStorageQueue(string queueName);

        ISimpleQueueBuilder<T> WithCustomSerializer(Func<T, string> serlializer);

        ISimpleQueueBuilder<T> WithCustomDeserializer(Func<string, T> deserializer);
    }

    public static class Builder
    {
        public static ISimpleQueueBuilder<T> CreateAQueueOf<T>()
        {
            return new SimpleQueueBuilder<T>();
        }
    }

    internal class SimpleQueueBuilder<T> : ISimpleQueueBuilder<T>
    {
        private string _connectionString = "UseDevelopmentStorage=true";
        private string _queueName;
        private Func<T, string> _customSerialize;
        private Func<string, T> _customDeserialize;



        internal SimpleQueueBuilder()
        {

        }        
        
        public ISimpleQueue<T> Build()
        {
            return new SimpleQueue<T>(_connectionString, _queueName, _customSerialize, _customDeserialize);
        }

        public ISimpleQueueBuilder<T> ConnectedToAccount(string connectionString)
        {
            _connectionString = connectionString;
            return this;
        }

        public ISimpleQueueBuilder<T> UsingStorageQueue(string queueName)
        {
            _queueName = queueName;
            return this;
        }

        public ISimpleQueueBuilder<T> WithCustomSerializer(Func<T, string> serlializer)
        {
            _customSerialize = serlializer;
            return this;
        }

        public ISimpleQueueBuilder<T> WithCustomDeserializer(Func<string, T> deserializer)
        {
            _customDeserialize = deserializer;
            return this;
        }
    }
}

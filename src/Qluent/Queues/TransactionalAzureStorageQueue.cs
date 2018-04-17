using Qluent.Serialization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;

namespace Qluent.Queues
{
    internal class TransactionalAzureStorageQueue<T> : SimpleAzureStorageQueue<T>, IEnlistmentNotification
    {
        private bool _deferEnqueueUntilCommitted;
        private List<T> _deferredMessages;

        internal TransactionalAzureStorageQueue(string connectionString, 
            string queueName, 
            BehaviorOnPoisonMessage behaviorOnPoisonMessage = BehaviorOnPoisonMessage.ThrowException, 
            string poisonQueueName = null,
            int considerPoisonAfterAttemptCount = 5,

            IStringMessageSerializer<T> _customStringSerializer = null,
            IBinaryMessageSerializer<T> _customBinarySerializer = null,

            int visibilityTimeout = 30000) 
                : base(connectionString, 
                      queueName, 
                      behaviorOnPoisonMessage, 
                      poisonQueueName, 
                      considerPoisonAfterAttemptCount,
                      _customStringSerializer,
                      _customBinarySerializer, 
                      visibilityTimeout)
        {
        }

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

        public override async Task PushAsync(T message)
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

        public override async Task PushAsync(IEnumerable<T> messages)
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
    }
}
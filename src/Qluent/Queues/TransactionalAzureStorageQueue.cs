using Qluent.Policies;
using Qluent.Serialization;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace Qluent.Queues
{
    internal class TransactionalAzureStorageQueue<T> : SimpleAzureStorageQueue<T>, IEnlistmentNotification
    {
        private bool _deferEnqueueUntilCommitted;
        private List<T> _deferredMessages;

        protected TransactionalAzureStorageQueue(
            IAzureStorageQueueSettings settings,
            IMessageTimeoutPolicy messageTimeoutPolicy,
            IPoisonMessageBehaviorPolicy poisonMessageBehaviorPolicy = null,
            IStringMessageSerializer<T> customStringSerializer = null,
            IBinaryMessageSerializer<T> customBinarySerializer = null) 
            : base(settings,
                   messageTimeoutPolicy,
                   poisonMessageBehaviorPolicy,
                   customStringSerializer,
                   customBinarySerializer)
        {

        }

        public new static async Task<TransactionalAzureStorageQueue<T>> CreateAsync(
            IAzureStorageQueueSettings settings,
            IMessageTimeoutPolicy messageTimeoutPolicy,
            IPoisonMessageBehaviorPolicy poisonMessageBehaviorPolicy = null,
            IStringMessageSerializer<T> customStringSerializer = null,
            IBinaryMessageSerializer<T> customBinarySerializer = null)
        {

            var queue = new TransactionalAzureStorageQueue<T>(settings,
                            messageTimeoutPolicy,
                            poisonMessageBehaviorPolicy,
                            customStringSerializer,
                            customBinarySerializer);

            await queue
                .InstantiateQueues()
                .ConfigureAwait(false);

            return queue;
        }

        public async void Commit(Enlistment enlistment)
        {
            foreach (var message in _deferredMessages)
            {
                await Enqueue(message, new CancellationToken())
                    .ConfigureAwait(false);
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
            await PushAsync(message, new CancellationToken())
                .ConfigureAwait(false);
        }

        public override async Task PushAsync(T message, CancellationToken token)
        {
            AttemptEnlistment();
            if (_deferEnqueueUntilCommitted)
            {
                _deferredMessages.Add(message);
            }
            else
            {
                await Enqueue(message, token)
                    .ConfigureAwait(false);
            }
        }

        public override async Task PushAsync(IEnumerable<T> messages)
        {
            await PushAsync(messages, new CancellationToken())
                .ConfigureAwait(false);
        }

        public override async Task PushAsync(IEnumerable<T> messages, CancellationToken token)
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
                    await Enqueue(message, token)
                        .ConfigureAwait(false);
                }
            }
        }
    }
}
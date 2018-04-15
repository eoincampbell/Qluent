using System;
using Qluent.Queues;

namespace Qluent.Builders
{
    internal class AzureStorageQueueBuilder<T> : IAzureStorageQueueBuilder<T>
    {
        private string _connectionString = "UseDevelopmentStorage=true";
        private string _queueName;
        private string _poisonQueueName;
        private bool _swallowExceptionOnPoisonMessage;
        private bool _transactionScopeAware;

        private Func<T, string> _customSerialize;
        private Func<string, T> _customDeserialize;

        public IAzureStorageQueue<T> Build()
        {
            var behavior = BehaviorOnPoisonMessage.ThrowException;
            if (_swallowExceptionOnPoisonMessage)
                behavior = BehaviorOnPoisonMessage.SwallowException;

            return _transactionScopeAware
                ? new TransactionalAzureStorageQueue<T>(_connectionString, _queueName, behavior, _poisonQueueName,
                    _customSerialize, _customDeserialize)
                : new SimpleAzureStorageQueue<T>(_connectionString, _queueName, behavior, _poisonQueueName,
                    _customSerialize, _customDeserialize);
        }

        public IAzureStorageQueueBuilder<T> ConnectedToAccount(string connectionString)
        {
            _connectionString = connectionString;
            return this;
        }

        public IAzureStorageQueueBuilder<T> UsingStorageQueue(string queueName)
        {
            _queueName = queueName;
            return this;
        }

        public IAzureStorageQueueBuilder<T> WithACustomSerializer(Func<T, string> serlializer)
        {
            _customSerialize = serlializer;
            return this;
        }

        public IAzureStorageQueueBuilder<T> WithACustomDeserializer(Func<string, T> deserializer)
        {
            _customDeserialize = deserializer;
            return this;
        }
        public IAzureStorageQueueBuilder<T> ThatIsTransactionScopeAware()
        {
            _transactionScopeAware = true;
            return this;
        }

        public IAzureStorageQueueBuilder<T> ThatSendsPoisonMessagesToThisQueue(string poisonQueueName)
        {
            _poisonQueueName = poisonQueueName;
            return this;
        }

        public IAzureStorageQueueBuilder<T> AndSwallowsExceptionsOnPoisonMessages()
        {
            _swallowExceptionOnPoisonMessage = true;
            return this;
        }
    }
}
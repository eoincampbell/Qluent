using System;
using Qluent.Queues;
using Qluent.Serialization;

namespace Qluent.Builders
{
    internal class AzureStorageQueueBuilder<T> : IAzureStorageQueueBuilder<T>
    {
        private string _connectionString = "UseDevelopmentStorage=true";
        private string _queueName;
        private string _poisonQueueName;

        private int _considerMessagesPoisonAfterAttempts = 10;
        private int _messageVisibilityTimeout = 30000;

        private bool _swallowExceptionOnPoisonMessage;
        private bool _transactionScopeAware;

        private IStringMessageSerializer<T> _customStringSerializer;
        private IBinaryMessageSerializer<T> _customBinarySerializer;

        public IAzureStorageQueue<T> Build()
        {
            var behavior = BehaviorOnPoisonMessage.ThrowException;

            if (_swallowExceptionOnPoisonMessage)
            {
                behavior = BehaviorOnPoisonMessage.SwallowException;
            }

            return _transactionScopeAware
                ? new TransactionalAzureStorageQueue<T>(_connectionString, _queueName, behavior, _poisonQueueName, _considerMessagesPoisonAfterAttempts,
                    _customStringSerializer, _customBinarySerializer, _messageVisibilityTimeout)
                : new SimpleAzureStorageQueue<T>(_connectionString, _queueName, behavior, _poisonQueueName, _considerMessagesPoisonAfterAttempts,
                    _customStringSerializer, _customBinarySerializer, _messageVisibilityTimeout);
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

        public IAzureStorageQueueBuilder<T> WithACustomStringSerializer(IStringMessageSerializer<T> customSerlializer)
        {
            _customStringSerializer = customSerlializer;
            return this;
        }

        public IAzureStorageQueueBuilder<T> WithACustomBinarySerializer(IBinaryMessageSerializer<T> customSerlializer)
        {
            _customBinarySerializer = customSerlializer;
            return this;
        }

        public IAzureStorageQueueBuilder<T> ThatIsTransactionScopeAware()
        {
            _transactionScopeAware = true;
            return this;
        }

        public IAzureStorageQueueBuilder<T> ThatSendsPoisonMessagesTo(string poisonQueueName, int afterAttempts = 3)
        {
            _poisonQueueName = poisonQueueName;
            _considerMessagesPoisonAfterAttempts = afterAttempts;
            return this;
        }

        public IAzureStorageQueueBuilder<T> AndSwallowExceptionsOnPoisonMessages()
        {
            _swallowExceptionOnPoisonMessage = true;
            return this;
        }

        public IAzureStorageQueueBuilder<T> WhereMessageVisibilityTimesOutAfter(int milliseconds)
        {
            _messageVisibilityTimeout = milliseconds;
            return this;
        }
    }
}
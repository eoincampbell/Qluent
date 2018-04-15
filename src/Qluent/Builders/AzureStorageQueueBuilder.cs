using System;
using Qluent.Queues;

namespace Qluent.Builders
{
    internal class AzureStorageQueueBuilder<T> : IAzureStorageQueueBuilder<T>
    {
        private string _connectionString = "UseDevelopmentStorage=true";
        private string _queueName;
        private string _poisonQueueName;
        private int _considerMessagesPoisonAfterAttempts;
        private bool _swallowExceptionOnPoisonMessage;
        private bool _transactionScopeAware;

        private Func<T, string> _customSerialize;
        private Func<string, T> _customDeserialize;
        private int _messageVisibilityTimeout = 30000;

        public IAzureStorageQueue<T> Build()
        {
            var behavior = BehaviorOnPoisonMessage.ThrowException;
            if (_swallowExceptionOnPoisonMessage)
                behavior = BehaviorOnPoisonMessage.SwallowException;

            return _transactionScopeAware
                ? new TransactionalAzureStorageQueue<T>(_connectionString, _queueName, behavior, _poisonQueueName, _considerMessagesPoisonAfterAttempts,
                    _customSerialize, _customDeserialize, _messageVisibilityTimeout)
                : new SimpleAzureStorageQueue<T>(_connectionString, _queueName, behavior, _poisonQueueName, _considerMessagesPoisonAfterAttempts,
                    _customSerialize, _customDeserialize, _messageVisibilityTimeout);
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

        public IAzureStorageQueueBuilder<T> ThatSendsPoisonMessagesTo(string poisonQueueName, int afterAttempts = 3)
        {
            _poisonQueueName = poisonQueueName;
            _considerMessagesPoisonAfterAttempts = afterAttempts;
            return this;
        }

        public IAzureStorageQueueBuilder<T> AndSwallowsExceptionsOnPoisonMessages()
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
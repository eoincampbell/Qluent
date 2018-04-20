using Qluent.Consumers;
using Qluent.Consumers.Handlers;
using Qluent.Consumers.Policies;

namespace Qluent
{
    public class MessageConsumerBuilder<T> : IMessageConsumerBuilder<T>
    {
        private readonly IMessageConsumerSettings _settings;
        private IAzureStorageQueue<T> _queue;
        private IMessageConsumerQueuePolingPolicy _queuePolingPolicy = new SetIntervalQueuePolingPolicy(5000);
        private IMessageHandler<T> _messageHandler;
        private IMessageHandler<T> _failedMessageHandler;
        private IMessageExceptionHandler<T> _exceptionHandler;

        internal MessageConsumerBuilder()
        {
            _settings = new MessageConsumerSettings();
        }


        public IMessageConsumerBuilder<T> UsingQueue(IAzureStorageQueue<T> queue)
        {
            _queue = queue;
            return this;
        }

        public IMessageConsumerBuilder<T> ThatHandlesMessagesUsing(IMessageHandler<T> messageHandler)
        {
            _messageHandler = messageHandler;
            return this;
        }

        public IMessageConsumerBuilder<T> AndHandlesFailedMessagesUsing(IMessageHandler<T> failedMessagehandler)
        {
            _failedMessageHandler = failedMessagehandler;
            return this;
        }

        public IMessageConsumerBuilder<T> AndHandlesExceptionsUsing(IMessageExceptionHandler<T> messageExceptionHandler)
        {
            _exceptionHandler = messageExceptionHandler;
            return this;
        }

        public IMessageConsumerBuilder<T> WithAQueuePolingPolicyOf(IMessageConsumerQueuePolingPolicy policy)
        {
            _queuePolingPolicy = policy;
            return this;
        }

        public IMessageConsumer<T> Build()
        {
            return new MessageConsumer<T>(
                _settings,
                _queue,
                _queuePolingPolicy,
                _messageHandler,
                _failedMessageHandler,
                _exceptionHandler);
        }
    }
}
namespace Qluent.Builders
{
    using Consumers;
    using Consumers.Handlers;
    using Consumers.Policies;
    using System;
    internal class MessageConsumerBuilder<T> : IMessageConsumerBuilder<T>
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

        public IMessageConsumerBuilder<T> ThatHandlesMessagesUsing(Func<IMessage<T>, bool> messageHandler)
        {
            _messageHandler = new InternalFunctionMessageHandler<T>(messageHandler);
            return this;
        }

        public IMessageConsumerBuilder<T> AndHandlesFailedMessagesUsing(IMessageHandler<T> failedMessagehandler)
        {
            _failedMessageHandler = failedMessagehandler;
            return this;
        }

        public IMessageConsumerBuilder<T> AndHandlesFailedMessagesUsing(Func<IMessage<T>, bool> failedMessageHandler)
        {
            _failedMessageHandler = new InternalFunctionMessageHandler<T>(failedMessageHandler);
            return this;
        }

        public IMessageConsumerBuilder<T> AndHandlesExceptionsUsing(IMessageExceptionHandler<T> messageExceptionHandler)
        {
            _exceptionHandler = messageExceptionHandler;
            return this;
        }

        public IMessageConsumerBuilder<T> AndHandlesExceptionsUsing(Func<IMessage<T>, Exception, bool> messageExceptionHandler)
        {
            _exceptionHandler = new InternalFunctionMessageExceptionHandler<T>(messageExceptionHandler);
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
namespace Qluent.Consumers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Handlers;
    using Policies;

    internal class MessageConsumer<T> : IMessageConsumer<T>
    {
        private readonly IMessageConsumerSettings _settings;
        private readonly IAzureStorageQueue<T> _queue;
        private readonly IMessageConsumerQueuePolingPolicy _queuePolingPolicy;
        private readonly IMessageHandler<T> _messageHandler;
        private readonly IMessageHandler<T> _failedMessageHandler;
        private readonly IMessageExceptionHandler<T> _exceptionHandler;
        

        internal MessageConsumer(
            IMessageConsumerSettings settings,
            IAzureStorageQueue<T> queue,
            IMessageConsumerQueuePolingPolicy queuePolingPolicy,
            IMessageHandler<T> messageHandler,
            IMessageHandler<T> failedMessageHandler,
            IMessageExceptionHandler<T> exceptionHandler
            )
        {
            _settings = settings;
            _queue = queue;
            _queuePolingPolicy = queuePolingPolicy;
            _messageHandler = messageHandler;
            _failedMessageHandler = failedMessageHandler;
            _exceptionHandler = exceptionHandler;
        }

        public async Task Start(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    IMessage<T> currentMessage;

                    while (true)
                    {
                        currentMessage = await _queue.GetAsync(cancellationToken);

                        if (currentMessage != null) break;

                        await Task.Delay(_queuePolingPolicy.NextDelay(false), cancellationToken);
                    }

                    _queuePolingPolicy.Reset();

                    try
                    {
                        var handlerSuccess = await _messageHandler.Handle(currentMessage, cancellationToken);

                        if (handlerSuccess)
                        {
                            await _queue.DeleteAsync(currentMessage, cancellationToken);
                        }
                        else
                        {
                            if (_failedMessageHandler == null) continue;

                            var failedHandlerSuccess = await _failedMessageHandler.Handle(currentMessage, cancellationToken);

                            if (failedHandlerSuccess)
                            {
                                await _queue.DeleteAsync(currentMessage, cancellationToken);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            await _exceptionHandler.Handle(currentMessage, ex, cancellationToken);
                        }
                        catch
                        {
                            //swallow & NLog.
                        }
                    }                    
                }
                catch (Exception ex)
                {
                    //Handle option Setting that states escaped exceptions should kill or continue
                }
            }
        }
    }
}

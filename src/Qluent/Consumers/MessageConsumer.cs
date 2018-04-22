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
            
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _queue = queue ?? throw new ArgumentNullException(nameof(queue)); 
            _queuePolingPolicy = queuePolingPolicy ?? throw new ArgumentNullException(nameof(queue));
            _messageHandler = messageHandler ?? throw new ArgumentNullException(nameof(messageHandler));
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

                        if (currentMessage != null)
                        {
                            _queuePolingPolicy.NextDelay(true);
                            break;
                        }

                        await Task.Delay(_queuePolingPolicy.NextDelay(false), cancellationToken);
                    }

                    

                    try
                    {
                        //attempt to process the message
                        var handlerSuccess = await _messageHandler.Handle(currentMessage, cancellationToken);

                        if (handlerSuccess ||   
                            _failedMessageHandler == null)
                        {
                            //if the handler is successful or if we don't a failed handler to fall back on
                            //then consider the message "handled" and remove the message
                            await _queue.DeleteAsync(currentMessage, cancellationToken);
                        }
                        else
                        {
                            await _failedMessageHandler.Handle(currentMessage, cancellationToken);
                            await _queue.DeleteAsync(currentMessage, cancellationToken);       
                        }
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            if (_failedMessageHandler != null)
                            {
                                await _exceptionHandler.Handle(currentMessage, ex, cancellationToken);
                            }
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

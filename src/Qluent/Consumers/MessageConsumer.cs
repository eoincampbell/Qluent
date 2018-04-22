namespace Qluent.Consumers
{
    using NLog;
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Handlers;
    using Policies;
    using Policies.ConsumerExceptionBehavior;

    internal class MessageConsumer<T> : IMessageConsumer<T>
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

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
            _logger.Info($"Consumer-{_settings.Id}: Starting");

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
                        _logger.Debug($"Consumer-{_settings.Id}: Handling Message: {currentMessage.MessageId}");
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
                            _logger.Debug($"Consumer-{_settings.Id}: Executing failed handler on message: {currentMessage.MessageId}");
                            await _failedMessageHandler.Handle(currentMessage, cancellationToken);
                            await _queue.DeleteAsync(currentMessage, cancellationToken);       
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex, $"Consumer-{_settings.Id}: Exception occured while processing {currentMessage}");
                        try
                        {
                            if (_exceptionHandler != null)
                            {
                                _logger.Debug($"Consumer-{_settings.Id}: Executing exception handler on message: {currentMessage.MessageId}");
                                await _exceptionHandler.Handle(currentMessage, ex, cancellationToken);
                            }
                        }
                        catch(Exception nex)
                        {
                            _logger.Error(nex, $"Consumer-{_settings.Id}: Exception occured while attempt to handle outer exception for {currentMessage}");
                        }
                    }                    
                }
                catch (Exception ex)
                {
                    if (_settings.Behavior == By.Exiting)
                    {
                        _logger.Fatal(ex, $"Consumer-{_settings.Id}: An exception occurred which FATALLY terminated the consumer.");
                        throw;
                    }
                    else if (_settings.Behavior == By.Continuing)
                    {
                        _logger.Error(ex, $"Consumer-{_settings.Id}: An exception occurred and the consumer continued iterating. A message may have been lost");
                    }
                }
            }

            _logger.Info($"Consumer-{_settings.Id}: Stopping");
        }
    }
}

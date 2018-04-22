namespace Qluent.Consumers.Handlers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    internal class InternalFunctionMessageExceptionHandler<T> : IMessageExceptionHandler<T>
    {
        private readonly Func<IMessage<T>, Exception, CancellationToken, Task<bool>> _function;

        internal InternalFunctionMessageExceptionHandler(Func<IMessage<T>, Exception, CancellationToken, Task<bool>> function)
        {
            _function = function;
        }

        public async Task<bool> Handle(IMessage<T> message, Exception exception, CancellationToken cancellationToken)
        {
            return await _function(message, exception, cancellationToken).ConfigureAwait(false);
        }
    }
}
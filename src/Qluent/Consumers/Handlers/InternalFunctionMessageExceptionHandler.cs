namespace Qluent.Consumers.Handlers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    internal class InternalFunctionMessageExceptionHandler<T> : IMessageExceptionHandler<T>
    {
        private readonly Func<IMessage<T>, Exception, bool> _function;

        internal InternalFunctionMessageExceptionHandler(Func<IMessage<T>, Exception, bool> function)
        {
            _function = function;
        }

        public Task<bool> Handle(IMessage<T> message, Exception exception, CancellationToken cancellationToken)
        {
            return Task.FromResult(_function(message, exception));
        }
    }
}
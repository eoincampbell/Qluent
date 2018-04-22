namespace Qluent.Consumers.Handlers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    internal class InternalFunctionMessageHandler<T> : IMessageHandler<T>
    {
        private readonly Func<IMessage<T>, CancellationToken, Task<bool>> _function;

        internal InternalFunctionMessageHandler(Func<IMessage<T>, CancellationToken, Task<bool>> function)
        {
            _function = function;
        }

        public async Task<bool> Handle(IMessage<T> message, CancellationToken cancellationToken)
        {
            return await _function(message, cancellationToken).ConfigureAwait(false);
        }
    }
}
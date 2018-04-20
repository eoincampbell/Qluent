namespace Qluent.Consumers.Handlers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    internal class InternalFunctionMessageHandler<T> : IMessageHandler<T>
    {
        private readonly Func<IMessage<T>, bool> _function;

        internal InternalFunctionMessageHandler(Func<IMessage<T>, bool> function)
        {
            _function = function;
        }

        public Task<bool> Handle(IMessage<T> message, CancellationToken cancellationToken)
        {
            return Task.FromResult(_function(message));
        }
    }
}
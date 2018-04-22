using Qluent.Consumers.Policies.ConsumerExceptionBehavior;

namespace Qluent.Consumers
{
    internal class MessageConsumerSettings : IMessageConsumerSettings
    {
        public string Id { get; set; }

        public By Behavior { get; set; }
    }
}

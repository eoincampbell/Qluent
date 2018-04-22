using System.IO;
using Qluent.Consumers.Policies.ConsumerExceptionBehavior;

namespace Qluent.Consumers
{
    internal interface IMessageConsumerSettings
    {
        string Id { get; set; }

        By Behavior { get; set; }
    }


}

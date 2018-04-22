namespace Qluent.Queues.Policies
{
    using System;

    internal class MessageTimeoutPolicy : IMessageTimeoutPolicy
    {
        public TimeSpan? InitialVisibilityDelay { get; set; } = null;
        public TimeSpan? VisibilityTimeout { get; set; } = null;
        public TimeSpan? TimeToLive { get; set; } = null;
    }
}

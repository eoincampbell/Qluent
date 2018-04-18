namespace Qluent.Policies
{
    using System;

    internal interface IMessageTimeoutPolicy
    {
        TimeSpan? InitialVisibilityDelay { get; set; }
        TimeSpan? VisibilityTimeout { get; set; }
        TimeSpan? TimeToLive { get; set; }
    }
}

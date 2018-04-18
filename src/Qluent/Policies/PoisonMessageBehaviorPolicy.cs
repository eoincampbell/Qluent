using Qluent.Queues;
using System;
using System.Collections.Generic;
using System.Text;

namespace Qluent.Policies
{
    public enum PoisonMessageBehavior
    {
        ThrowingExceptions,
        SwallowingExceptions
    }

    internal interface IPoisonMessageBehaviorPolicy
    {
        PoisonMessageBehavior PoisonMessageBehavior { get; set; }
        int PoisonMessageDequeueAttemptThreshold { get; set; }
        string PoisonMessageStorageQueueName { get; set; }
    }

    internal class PoisonMessageBehaviorPolicy : IPoisonMessageBehaviorPolicy
    {
        public PoisonMessageBehavior PoisonMessageBehavior { get; set; } = PoisonMessageBehavior.ThrowingExceptions;
        public int PoisonMessageDequeueAttemptThreshold { get; set; } = 5;
        public string PoisonMessageStorageQueueName { get; set; } = null;
    }

    internal interface IAzureStorageQueueSettings
    {
        string ConnectionString { get; set; }
        string StorageQueueName { get; set; }
    }

    internal class AzureStorageQueueSettings : IAzureStorageQueueSettings
    {
        public string ConnectionString { get; set; } = "UseDevelopmentStorage=true";
        public string StorageQueueName { get; set; } = null;
    }

    internal interface IMessageTimeoutPolicy
    {
        TimeSpan? InitialVisibilityDelay { get; set; }
        TimeSpan? VisibilityTimeout { get; set; }
        TimeSpan? TimeToLive { get; set; }
    }

    internal class MessageTimeoutPolicy : IMessageTimeoutPolicy
    {
        public TimeSpan? InitialVisibilityDelay { get; set; } = null;

        public TimeSpan? VisibilityTimeout { get; set; } = null;

        public TimeSpan? TimeToLive { get; set; } = null;
    }
}

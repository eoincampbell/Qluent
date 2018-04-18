namespace Qluent.Policies
{
    using Qluent.Policies.PoisonMessageBehavior;

    internal interface IPoisonMessageBehaviorPolicy
    {
        By PoisonMessageBehavior { get; set; }
        int PoisonMessageDequeueAttemptThreshold { get; set; }
        string PoisonMessageStorageQueueName { get; set; }
    }
}

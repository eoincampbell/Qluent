namespace Qluent.Policies
{
    using PoisonMessageBehavior;

    internal interface IPoisonMessageBehaviorPolicy
    {
        By PoisonMessageBehavior { get; set; }
        int PoisonMessageDequeueAttemptThreshold { get; set; }
        string PoisonMessageStorageQueueName { get; set; }
    }
}

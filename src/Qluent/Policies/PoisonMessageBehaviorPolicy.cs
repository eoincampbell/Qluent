namespace Qluent.Policies
{
    using PoisonMessageBehavior;

    internal class PoisonMessageBehaviorPolicy : IPoisonMessageBehaviorPolicy
    {
        public By PoisonMessageBehavior { get; set; } = By.ThrowingExceptions;
        public int PoisonMessageDequeueAttemptThreshold { get; set; } = 5;
        public string PoisonMessageStorageQueueName { get; set; } = null;
    }
}

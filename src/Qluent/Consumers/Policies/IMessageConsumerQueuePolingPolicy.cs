namespace Qluent.Consumers.Policies
{
    using System;

    /// <summary>
    /// An interface which defines what the next delay should be before polling the storage queue.
    /// 
    /// The interval can be modified based on whether or not the last attempt to obtain a message was successful.
    /// </summary>
    public interface IMessageConsumerQueuePolingPolicy
    {
        /// <summary>
        /// Gets the next delay interval
        /// </summary>
        /// <param name="lastOperationSucceeded">if set to <c>true</c> the last dequeue operation succeeded.</param>
        /// <returns>A <see cref="TimeSpan"/> specifying the next delay</returns>
        TimeSpan NextDelay(bool lastOperationSucceeded);

        /// <summary>
        /// Resets the policy to its default delay interval
        /// </summary>
        void Reset();
    }
}
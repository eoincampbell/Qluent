using System;

namespace Qluent.Consumers.Policies
{
    /// <summary>
    /// An <see cref="IMessageConsumerQueuePolingPolicy"/> which always returns a static interval regardless of previous success/failure
    /// </summary>
    /// <seealso cref="Qluent.Consumers.Policies.IMessageConsumerQueuePolingPolicy" />
    public class SetIntervalQueuePolingPolicy : IMessageConsumerQueuePolingPolicy
    {
        private readonly double _intervalMilliseconds;

        /// <summary>
        /// Initializes a new instance of the <see cref="SetIntervalQueuePolingPolicy"/> class.
        /// </summary>
        /// <param name="intervalMilliseconds">The interval in milliseconds.</param>
        public SetIntervalQueuePolingPolicy(double intervalMilliseconds)
        {
            _intervalMilliseconds = intervalMilliseconds;
        }

        /// <summary>
        /// Gets the next delay interval
        /// </summary>
        /// <param name="lastOperationSucceeded">if set to <c>true</c> the last dequeue operation succeeded.</param>
        /// <returns>
        /// A <see cref="T:System.TimeSpan" /> specifying the next delay
        /// </returns>
        public TimeSpan NextDelay(bool lastOperationSucceeded)
        {
            return TimeSpan.FromMilliseconds(_intervalMilliseconds);
        }

        /// <summary>
        /// Resets the policy to its default delay interval
        /// </summary>
        public void Reset()
        {
            
        }
    }
}
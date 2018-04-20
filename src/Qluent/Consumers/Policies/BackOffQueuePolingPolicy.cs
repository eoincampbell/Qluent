using System;

namespace Qluent.Consumers.Policies
{
    /// <summary>
    /// An <see cref="IMessageConsumerQueuePolingPolicy"/> which backs off poling the queue on failures using the following seconds interval
    /// 
    /// 1s, 1s, 1s, 1s, 2s, 2s, 4s, 4s, 8s, 15s, 30s, 60s
    /// 
    /// On success, the interval will move left. On failure the interval will move right to a max of 60 seconds
    /// 
    /// The interval can be reset to 1 second by calling <code>Reset()</code>
    /// </summary>
    /// <seealso cref="Qluent.Consumers.Policies.IMessageConsumerQueuePolingPolicy" />
    public class BackOffQueuePolingPolicy : IMessageConsumerQueuePolingPolicy
    {
        private readonly int[] _seconds = {1, 1, 1, 1, 2, 2, 4, 4, 8, 15, 30, 60};
        private int _index = 0;

        /// <summary>
        /// Gets the next delay interval
        /// </summary>
        /// <param name="lastOperationSucceeded">if set to <c>true</c> the last dequeue operation succeeded.</param>
        /// <returns>
        /// A <see cref="T:System.TimeSpan" /> specifying the next delay
        /// </returns>
        public TimeSpan NextDelay(bool lastOperationSucceeded)
        {
            if (lastOperationSucceeded)
            {
                if (_index > 0) _index--;
            }
            else
            {
                if (_index < 11) _index++;
            }

            return TimeSpan.FromSeconds(_seconds[_index]);
        }

        /// <summary>
        /// Resets the policy to its default delay interval
        /// </summary>
        public void Reset()
        {
            _index= 0;
        }
    }
}
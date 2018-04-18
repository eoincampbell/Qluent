namespace Qluent
{
    /// <summary>
    /// A Message Wrapper for messages removed from an Azure Storage Queue
    /// </summary>
    /// <typeparam name="T">The payload type</typeparam>
    public class Message<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Message{T}"/> class.
        /// </summary>
        /// <param name="messageId">The message identifier.</param>
        /// <param name="popReceipt">The pop receipt.</param>
        /// <param name="value">The value.</param>
        internal Message(string messageId, string popReceipt, T value)
        {
            MessageId = messageId;
            PopReceipt = popReceipt;
            Value = value;
        }
        /// <summary>
        /// Gets the message identifier.
        /// </summary>
        /// <value>
        /// The message identifier.
        /// </value>
        public string MessageId { get; }
        /// <summary>
        /// Gets the pop receipt.
        /// </summary>
        /// <value>
        /// The pop receipt.
        /// </value>
        public string PopReceipt { get; }
        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public T Value { get; }
    }
}

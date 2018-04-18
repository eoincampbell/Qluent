namespace Qluent
{
    /// <summary>
    /// A Message Wrapper for messages removed from an Azure Storage Queue
    /// </summary>
    /// <typeparam name="T">The payload type</typeparam>
    public interface IMessage<out T>
    {
        /// <summary>
        /// Gets the message identifier.
        /// </summary>
        /// <value>
        /// The message identifier.
        /// </value>
        string MessageId { get; }
        /// <summary>
        /// Gets the pop receipt.
        /// </summary>
        /// <value>
        /// The pop receipt.
        /// </value>
        string PopReceipt { get; }
        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        T Value { get; }
    }
}
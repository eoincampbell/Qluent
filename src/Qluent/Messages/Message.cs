namespace Qluent.Messages
{
    internal class Message<T> : IMessage<T>
    {
        internal Message(string messageId, string popReceipt, T value)
        {
            MessageId = messageId;
            PopReceipt = popReceipt;
            Value = value;
        }

        public string MessageId { get; }

        public string PopReceipt { get; }

        public T Value { get; }
    }
}

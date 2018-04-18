namespace Qluent.Serialization
{
    /// <inheritdoc />
    /// <summary>
    /// An interface for specifying a Custom Serializtion/Deserialization implementation to convert your message to a <see cref="T:System.String" />
    /// </summary>
    /// <typeparam name="T">The payload object type</typeparam>
    /// <seealso cref="T:System.String" />
    public interface IStringMessageSerializer<T> : IMessageSerializer<T, string>
    {

    }
}

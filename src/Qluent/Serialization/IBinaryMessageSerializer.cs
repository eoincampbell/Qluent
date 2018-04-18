namespace Qluent.Serialization
{
    /// <summary>
    /// An interface for specifying a Custom Serializtion/Deserialization implementation to convert your message to a <see cref="System.Byte[]"/>
    /// </summary>
    /// <typeparam name="T">The payload object type</typeparam>
    /// <seealso cref="Qluent.Serialization.IMessageSerializer{T, System.Byte[]}" />
    public interface IBinaryMessageSerializer<T> : IMessageSerializer<T, byte[]>
    {

    }
}

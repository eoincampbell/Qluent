namespace Qluent.Serialization
{
    /// <summary>
    /// An interface for specifying a Custom Serializtion/Deserialization implementation to convert your message to a <see cref="System.String[]"/>
    /// </summary>
    /// <typeparam name="T">The payload object type</typeparam>
    /// <seealso cref="Qluent.Serialization.IMessageSerializer{T, System.String}" />
    public interface IStringMessageSerializer<T> : IMessageSerializer<T, string>
    {

    }
}

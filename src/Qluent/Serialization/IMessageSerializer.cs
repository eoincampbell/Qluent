namespace Qluent.Serialization
{
    /// <summary>
    /// An base interface for specifying a Custom Serializtion/Deserialization implementation to convert your message binary/string/>
    /// </summary>
    /// <typeparam name="T">The payload object type</typeparam>
    /// <typeparam name="TK">The serialized data format</typeparam>
    public interface IMessageSerializer<T, TK>
    {
        TK Serialize(T entity);
        T Deserialize(TK message);
    }
}

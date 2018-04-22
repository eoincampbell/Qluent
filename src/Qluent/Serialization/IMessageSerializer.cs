namespace Qluent.Serialization
{
    /// <summary>
    /// An base interface for specifying a Custom Serializtion/Deserialization implementation to convert your message binary/string/>
    /// </summary>
    /// <typeparam name="T">The payload object type</typeparam>
    /// <typeparam name="TK">The serialized data format</typeparam>
    public interface IMessageSerializer<T, TK>
    {
        /// <summary>
        /// Serializes the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>The serialized message</returns>
        TK Serialize(T entity);
        /// <summary>
        /// Deserializes the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>The deserialized entity</returns>
        T Deserialize(TK message);
    }
}

namespace Qluent.Serialization
{
    using Newtonsoft.Json;

    internal class DefaultMessageSerializer<T> : IStringMessageSerializer<T>
    {
        public T Deserialize(string message)
        {
            return JsonConvert.DeserializeObject<T>(message);
        }

        public string Serialize(T entity)
        {
            return JsonConvert.SerializeObject(entity);
        }
    }
}

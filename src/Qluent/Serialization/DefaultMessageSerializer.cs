using Newtonsoft.Json;

namespace Qluent.Serialization
{
    public class DefaultMessageSerializer<T> : IStringMessageSerializer<T>
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

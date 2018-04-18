using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Qluent.NetCore.Tests.Stubs;
using Qluent.Serialization;

namespace Qluent.NetFramework.Tests.Serializers
{
    public class CustomBinarySerializer : IBinaryMessageSerializer<Person>
    {
        public Person Deserialize(byte[] messageBytes)
        {
            using (var ms = new MemoryStream(messageBytes))
            {
                var bf = new BinaryFormatter();
                var obj = bf.Deserialize(ms);
                return obj as Person;
            }
        }

        public byte[] Serialize(Person entity)
        {
            using (var ms = new MemoryStream())
            {
                var bf = new BinaryFormatter();
                bf.Serialize(ms, entity);
                return ms.ToArray();
            }

        }
    }
}
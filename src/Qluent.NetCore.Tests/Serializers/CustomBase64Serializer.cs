using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Qluent.NetCore.Tests.Stubs;
using Qluent.Serialization;

namespace Qluent.NetCore.Tests.Helpers
{
    public class CustomBase64Serializer : IStringMessageSerializer<Person>
    {
        public Person Deserialize(string message)
        {
            var bytes = Convert.FromBase64String(message);
            using (var ms = new MemoryStream(bytes))
            {
                var bf = new BinaryFormatter();
                var obj = bf.Deserialize(ms);
                return obj as Person;
            }
        }

        public string Serialize(Person entity)
        {
            using (var ms = new MemoryStream())
            {
                var bf = new BinaryFormatter();
                bf.Serialize(ms, entity);
                return Convert.ToBase64String(ms.ToArray());
            }

        }
    }
}
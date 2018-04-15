using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Qluent.NetFramework.Tests.Stubs;

namespace Qluent.NetFramework.Tests.Helpers
{
    public static class CustomSerializationHelper
    {
        public static Func<Person, string> Base64Serializer = person =>
        {
            using (var ms = new MemoryStream())
            {
                var bf = new BinaryFormatter();
                bf.Serialize(ms, person);
                return Convert.ToBase64String(ms.ToArray());
            }
        };

        public static Func<string, Person> Base64Deserializer = str =>
        {
            var bytes = Convert.FromBase64String(str);
            using (var ms = new MemoryStream(bytes))
            {
                var bf = new BinaryFormatter();
                var obj = bf.Deserialize(ms);
                return obj as Person;
            }
        };
    }
}
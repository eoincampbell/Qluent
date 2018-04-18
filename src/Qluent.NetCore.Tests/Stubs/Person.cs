using System;

namespace Qluent.NetCore.Tests.Stubs
{
    [Serializable]
    public class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }

        public static Person Create()
        {
            return new Person()
            {
                Name = "John",
                Age = 30
            };
        }
    }
}
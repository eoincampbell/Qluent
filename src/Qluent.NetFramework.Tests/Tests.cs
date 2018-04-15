using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using System.Transactions;
using NUnit.Framework;

namespace Qluent.NetFramework.Tests
{
    [TestFixture]
    public class Tests
    {
        [Test]
        public async Task CreateBasicTestStorageQueue()
        {
            var q = Builder
                .CreateAQueueOf<Person>()
                .UsingStorageQueue("my-test-queue")
                .Build();

            await q.PurgeAsync();
        }

        [Test]
        public async Task CreateBasicCustomAccountStorageQueue()
        {
            var q = Builder
                .CreateAQueueOf<Person>()
                .ConnectedToAccount("UseDevelopmentStorage=true")
                .UsingStorageQueue("my-test-queue")
                .Build();

            await q.PurgeAsync();
        }


        [Test]
        public async Task AddAMessage()
        {
            var q = Builder
                .CreateAQueueOf<Person>()
                .ConnectedToAccount("UseDevelopmentStorage=true")
                .UsingStorageQueue("my-test-queue")
                .Build();

            var message = Person.Create();

            await q.PurgeAsync();

            await q.PushAsync(message);

            int? count = await q.CountAsync();

            Assert.AreEqual(count, 1);

            await q.PurgeAsync();
        }


        [Test]
        public async Task GetAMessage()
        {
            var q = Builder
                .CreateAQueueOf<Person>()
                .ConnectedToAccount("UseDevelopmentStorage=true")
                .UsingStorageQueue("my-test-queue")
                .Build();

            var message = Person.Create();

            await q.PurgeAsync();

            await q.PushAsync(message);

            int? count = await q.CountAsync();

            Assert.AreEqual(count, 1);

            var response = await q.PopAsync();

            Assert.AreEqual("John", response.Name);

            int? count2 = await q.CountAsync();

            Assert.AreEqual(count2, 0);
        }

        [Test]
        public async Task AddAMessageCustomSerialization()
        {
            var q = Builder
                .CreateAQueueOf<Person>()
                .ConnectedToAccount("UseDevelopmentStorage=true")
                .UsingStorageQueue("my-test-queue")
                .WithCustomSerializer(CustomSerializationHelper.Base64Serializer)
                .Build();


            var message = Person.Create();

            await q.PurgeAsync();

            await q.PushAsync(message);

            int? count = await q.CountAsync();

            Assert.AreEqual(count, 1);

            await q.PurgeAsync();
        }

        [Test]
        public async Task GetAMessageCustomDeserialization()
        {
            var q = Builder
                .CreateAQueueOf<Person>()
                .ConnectedToAccount("UseDevelopmentStorage=true")
                .UsingStorageQueue("my-test-queue")
                .WithCustomSerializer(CustomSerializationHelper.Base64Serializer)
                .WithCustomDeserializer(CustomSerializationHelper.Base64Deserializer)
                .Build();

            var message = Person.Create();

            await q.PurgeAsync();

            await q.PushAsync(message);

            int? count = await q.CountAsync();

            Assert.AreEqual(count, 1);

            var response = await q.PopAsync();

            Assert.AreEqual("John", response.Name);

            int? count2 = await q.CountAsync();

            Assert.AreEqual(count2, 0);
        }


        [Test]
        public async Task AddAMessageTransactionallyAndCommit()
        {
            var q = Builder
                .CreateAQueueOf<Person>()
                .ConnectedToAccount("UseDevelopmentStorage=true")
                .UsingStorageQueue("my-test-queue")
                .Build();

            var message = Person.Create();

            await q.PurgeAsync();

            using (var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                await q.PushAsync(message);

                int? preCount = await q.CountAsync();
                Assert.AreEqual(preCount, 0);

                ts.Complete();
            }

            await Task.Delay(5000);

            int? postCount = await q.CountAsync();
            Assert.AreEqual(postCount, 1);

            await q.PurgeAsync();
        }

        [Test]
        public async Task AddAMessageTransactionallyAndRollback()
        {
            var q = Builder
                .CreateAQueueOf<Person>()
                .ConnectedToAccount("UseDevelopmentStorage=true")
                .UsingStorageQueue("my-test-queue")
                .Build();

            var message = Person.Create();

            await q.PurgeAsync();

            using (var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            { 
                await q.PushAsync(message);

                int? preCount = await q.CountAsync();
                Assert.AreEqual(preCount, 0);

            }

            await Task.Delay(5000);

            int? postCount = await q.CountAsync();
            Assert.AreEqual(postCount, 0);

            await q.PurgeAsync();
        }
    }

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
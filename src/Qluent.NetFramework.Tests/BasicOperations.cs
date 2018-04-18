using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using System.Transactions;
using Newtonsoft.Json;
using NUnit.Framework;
using Qluent.NetFramework.Tests.Helpers;
using Qluent.NetFramework.Tests.Stubs;

namespace Qluent.NetFramework.Tests
{
    [TestFixture]
    public class BasicOperations
    {
        [Test]
        public async Task Given_a_simple_queue_When_a_message_is_pushed_Then_a_message_should_be_added_to_the_queue()
        {
            var q = Builder
                .CreateAQueueOf<Person>()
                .ConnectedToAccount("UseDevelopmentStorage=true")
                .UsingStorageQueue("my-test-queue")
                .Build();

            var message = Person.Create();

            await q.PurgeAsync();
            await q.PushAsync(message);
            Assert.AreEqual(1, await q.CountAsync());
        }


        [Test]
        public async Task Given_a_simple_queue_When_a_message_is_popped_Then_a_message_should_be_removed_from_the_queue()
        {
            var q = Builder
                .CreateAQueueOf<Person>()
                .ConnectedToAccount("UseDevelopmentStorage=true")
                .UsingStorageQueue("my-test-queue")
                .Build();

            var message = Person.Create();

            await q.PurgeAsync();
            await q.PushAsync(message);
            Assert.AreEqual(1, await q.CountAsync());

            var response = await q.PopAsync();

            Assert.AreEqual("John", response.Name);
            Assert.AreEqual(0, await q.CountAsync());
        }

        [Test]
        public async Task Given_a_simple_queue_When_multiple_messages_are_popped_Then_multiple_messages_should_be_removed_from_the_queue()
        {
            var q = Builder
                .CreateAQueueOf<Person>()
                .ConnectedToAccount("UseDevelopmentStorage=true")
                .UsingStorageQueue("my-test-queue")
                .Build();

            var message = Person.Create();

            await q.PurgeAsync();
            await q.PushAsync(message);
            await q.PushAsync(message);
            await q.PushAsync(message);
            Assert.AreEqual(3, await q.CountAsync());

            var response = await q.PopAsync(3);
            Assert.AreEqual(3, response.Count());
        }


        


    }
}
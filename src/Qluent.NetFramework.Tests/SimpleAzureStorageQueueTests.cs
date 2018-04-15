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
    public class SimpleAzureStorageQueueTests
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

        [Test]
        public async Task Given_a_simple_queue_When_a_message_is_pushed_using_a_custom_serializer_Then_a_message_should_be_added_to_the_queue()
        {
            var q = Builder
                .CreateAQueueOf<Person>()
                .ConnectedToAccount("UseDevelopmentStorage=true")
                .UsingStorageQueue("my-test-queue")
                .WithACustomSerializer(CustomSerializationHelper.Base64Serializer)
                .Build();

            var message = Person.Create();

            await q.PurgeAsync();
            await q.PushAsync(message);    
            Assert.AreEqual(1, await q.CountAsync());
        }

        [Test]
        public async Task Given_a_simple_queue_When_a_message_is_popped_using_a_custom_deserializer_Then_a_message_should_be_removed_from_the_queue()
        {
            var q = Builder
                .CreateAQueueOf<Person>()
                .ConnectedToAccount("UseDevelopmentStorage=true")
                .UsingStorageQueue("my-test-queue")
                .WithACustomSerializer(CustomSerializationHelper.Base64Serializer)
                .WithACustomDeserializer(CustomSerializationHelper.Base64Deserializer)
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
        public async Task Given_a_simple_queue_When_a_poison_message_is_popped_and_a_poison_queue_is_defined_and_exceptions_should_be_thrown_Then_a_message_should_be_sent_to_the_poison_queue_and_an_exception_thrown()
        {
            var poisonQueue = Builder
                .CreateAQueueOf<string>()
                .UsingStorageQueue("my-poison-queue")
                .Build();

            await poisonQueue.PurgeAsync();

            var personQueue = Builder
                .CreateAQueueOf<Person>()
                .UsingStorageQueue("my-test-queue")
                .WithACustomSerializer(CustomSerializationHelper.Base64Serializer)
                .Build();

            var person = Person.Create();
            await personQueue.PurgeAsync();
            await personQueue.PushAsync(person);
            Assert.AreEqual(1, await personQueue.CountAsync());

            var jobQueue = Builder
                .CreateAQueueOf<Job>()
                .UsingStorageQueue("my-test-queue")
                .ThatSendsPoisonMessagesToThisQueue("my-poison-queue")
                .Build();

            Assert.ThrowsAsync<JsonReaderException>(async () => await jobQueue.PopAsync());

            Assert.AreEqual(1, await poisonQueue.CountAsync());
        }

        [Test]
        public async Task Given_a_simple_queue_When_a_poison_message_is_popped_and_a_poison_queue_is_not_defined_and_exceptions_should_be_thrown_Then_an_exception_should_be_thrown()
        {
            var personQueue = Builder
                .CreateAQueueOf<Person>()
                .UsingStorageQueue("my-test-queue")
                .WithACustomSerializer(CustomSerializationHelper.Base64Serializer)
                .Build();

            var person = Person.Create();
            await personQueue.PurgeAsync();
            await personQueue.PushAsync(person);
            Assert.AreEqual(1, await personQueue.CountAsync());

            var jobQueue = Builder
                .CreateAQueueOf<Job>()
                .UsingStorageQueue("my-test-queue")
                .Build();

            Assert.ThrowsAsync<JsonReaderException>(async () => await jobQueue.PopAsync());
        }

        [Test]
        public async Task Given_a_simple_queue_When_a_poison_message_is_popped_and_a_poison_queue_is_defined_and_exceptions_should_be_swallowed_Then_a_message_should_be_sent_to_the_poison_queue()
        {
            var poisonQueue = Builder
                .CreateAQueueOf<string>()
                .UsingStorageQueue("my-poison-queue")
                .Build();

            await poisonQueue.PurgeAsync();


            var personQueue = Builder
                .CreateAQueueOf<Person>()
                .UsingStorageQueue("my-test-queue")
                .WithACustomSerializer(CustomSerializationHelper.Base64Serializer)
                .Build();

            var person = Person.Create();
            await personQueue.PurgeAsync();
            await personQueue.PushAsync(person);
            Assert.AreEqual(1, await personQueue.CountAsync());

            var jobQueue = Builder
                .CreateAQueueOf<Job>()
                .UsingStorageQueue("my-test-queue")
                .ThatSendsPoisonMessagesToThisQueue("my-poison-queue")
                .AndSwallowsExceptionsOnPoisonMessages()
                .Build();

            var nullJob = await jobQueue.PopAsync();
            Assert.IsNull(nullJob);

            Assert.AreEqual(1, await poisonQueue.CountAsync());
        }

        [Test]
        public async Task Given_a_simple_queue_When_a_poison_message_is_popped_and_a_poison_queue_is_not_defined_and_exceptions_should_be_swallowed_Then_nothing_should_happen()
        {
            var personQueue = Builder
                .CreateAQueueOf<Person>()
                .UsingStorageQueue("my-test-queue")
                .WithACustomSerializer(CustomSerializationHelper.Base64Serializer)
                .Build();

            var person = Person.Create();
            await personQueue.PurgeAsync();
            await personQueue.PushAsync(person);
            Assert.AreEqual(1, await personQueue.CountAsync());

            var jobQueue = Builder
                .CreateAQueueOf<Job>()
                .UsingStorageQueue("my-test-queue")
                .AndSwallowsExceptionsOnPoisonMessages()
                .Build();

            var nullJob = await jobQueue.PopAsync();
            Assert.IsNull(nullJob);
        }
    }
}
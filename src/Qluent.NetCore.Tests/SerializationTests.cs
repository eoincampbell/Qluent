using NUnit.Framework;
using Qluent.NetCore.Tests.Stubs;
using System.Threading.Tasks;
using Qluent.NetCore.Tests.Serializers;

namespace Qluent.NetCore.Tests
{
    [TestFixture]
    public class SerializationTests
    {

        [Test]
        public async Task Given_a_simple_queue_When_a_message_is_pushed_using_a_custom_string_serializer_Then_a_message_should_be_added_to_the_queue()
        {
            var q = Builder
                .CreateAQueueOf<Person>()
                .ConnectedToAccount("UseDevelopmentStorage=true")
                .UsingStorageQueue("my-test-queue")
                .WithACustomSerializer(new CustomBase64Serializer())
                .Build();

            var message = Person.Create();

            await q.PurgeAsync();
            await q.PushAsync(message);
            Assert.AreEqual(1, await q.CountAsync());
        }

        [Test]
        public async Task Given_a_simple_queue_When_a_message_is_popped_using_a_custom_string_deserializer_Then_a_message_should_be_removed_from_the_queue()
        {
            var q = Builder
                .CreateAQueueOf<Person>()
                .ConnectedToAccount("UseDevelopmentStorage=true")
                .UsingStorageQueue("my-test-queue")
                .WithACustomSerializer(new CustomBase64Serializer())
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
        public async Task Given_a_simple_queue_When_a_message_is_pushed_using_a_custom_binary_serializer_Then_a_message_should_be_added_to_the_queue()
        {
            var q = Builder
                .CreateAQueueOf<Person>()
                .ConnectedToAccount("UseDevelopmentStorage=true")
                .UsingStorageQueue("my-test-queue")
                .WithACustomSerializer(new CustomBinarySerializer())
                .Build();

            var message = Person.Create();

            await q.PurgeAsync();
            await q.PushAsync(message);
            Assert.AreEqual(1, await q.CountAsync());
        }

        [Test]
        public async Task Given_a_simple_queue_When_a_message_is_popped_using_a_custom_binary_deserializer_Then_a_message_should_be_removed_from_the_queue()
        {
            var q = Builder
                .CreateAQueueOf<Person>()
                .ConnectedToAccount("UseDevelopmentStorage=true")
                .UsingStorageQueue("my-test-queue")
                .WithACustomSerializer(new CustomBinarySerializer())
                .Build();

            var message = Person.Create();

            await q.PurgeAsync();
            await q.PushAsync(message);
            Assert.AreEqual(1, await q.CountAsync());

            var response = await q.PopAsync();
            Assert.AreEqual("John", response.Name);
            Assert.AreEqual(0, await q.CountAsync());
        }


    }
}

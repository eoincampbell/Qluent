using Newtonsoft.Json;
using NUnit.Framework;
using Qluent.NetCore.Tests.Stubs;
using Qluent.Queues.Policies.PoisonMessageBehavior;
using System;
using System.Threading.Tasks;
using Qluent.NetCore.Tests.Serializers;

namespace Qluent.NetCore.Tests
{
    [TestFixture]
    public class PoisonMessageTests
    {
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
                .WithACustomSerializer(new CustomBase64Serializer())
                .Build();

            var person = Person.Create();
            await personQueue.PurgeAsync();
            await personQueue.PushAsync(person);
            Assert.AreEqual(1, await personQueue.CountAsync());

            var jobQueue = Builder
                .CreateAQueueOf<Job>()
                .UsingStorageQueue("my-test-queue")
                .ThatConsidersMessagesPoisonAfter(1)
                .AndSendsPoisonMessagesTo("my-poison-queue")
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
                .WithACustomSerializer(new CustomBase64Serializer())
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
                .WithACustomSerializer(new CustomBase64Serializer())
                .Build();

            var person = Person.Create();
            await personQueue.PurgeAsync();
            await personQueue.PushAsync(person);
            Assert.AreEqual(1, await personQueue.CountAsync());

            var jobQueue = Builder
                .CreateAQueueOf<Job>()
                .UsingStorageQueue("my-test-queue")
                .ThatConsidersMessagesPoisonAfter(1)
                .AndSendsPoisonMessagesTo("my-poison-queue")
                .AndHandlesExceptionsOnPoisonMessages(By.SwallowingExceptions)
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
                .WithACustomSerializer(new CustomBase64Serializer())
                .Build();

            var person = Person.Create();
            await personQueue.PurgeAsync();
            await personQueue.PushAsync(person);
            Assert.AreEqual(1, await personQueue.CountAsync());

            var jobQueue = Builder
                .CreateAQueueOf<Job>()
                .UsingStorageQueue("my-test-queue")
                .AndHandlesExceptionsOnPoisonMessages(By.SwallowingExceptions)
                .Build();

            var nullJob = await jobQueue.PopAsync();
            Assert.IsNull(nullJob);
        }

        [Test]
        public async Task Given_a_simple_queue_When_a_poison_message_is_popped_with_an_attempt_threshold_and_a_poison_queue_is_defined_and_exceptions_should_be_swallowed_Then_a_message_should_be_sent_to_the_poison_queue_after_three_attempts()
        {
            var poisonQueue = Builder
                .CreateAQueueOf<string>()
                .UsingStorageQueue("my-poison-queue")
                .Build();

            await poisonQueue.PurgeAsync();


            var personQueue = Builder
                .CreateAQueueOf<Person>()
                .UsingStorageQueue("my-test-queue")
                .WithACustomSerializer(new CustomBase64Serializer())
                .Build();

            var person = Person.Create();
            await personQueue.PurgeAsync();
            await personQueue.PushAsync(person);
            Assert.AreEqual(1, await personQueue.CountAsync());


            var jobQueue = Builder
                .CreateAQueueOf<Job>()
                .UsingStorageQueue("my-test-queue")
                .ThatKeepsMessagesInvisibleAfterDequeuingFor(TimeSpan.FromMilliseconds(500))
                .ThatConsidersMessagesPoisonAfter(3)
                .AndSendsPoisonMessagesTo("my-poison-queue")
                .AndHandlesExceptionsOnPoisonMessages(By.SwallowingExceptions)
                .Build();

            var nullJob = await jobQueue.PopAsync();
            Assert.IsNull(nullJob);
            Assert.AreEqual(0, await poisonQueue.CountAsync());
            await Task.Delay(1000);

            nullJob = await jobQueue.PopAsync();
            Assert.IsNull(nullJob);
            Assert.AreEqual(0, await poisonQueue.CountAsync());
            await Task.Delay(1000);

            nullJob = await jobQueue.PopAsync();
            Assert.IsNull(nullJob);
            Assert.AreEqual(1, await poisonQueue.CountAsync());
        }
    }
}

using System;
using System.Threading.Tasks;
using NUnit.Framework;
using Qluent.NetCore.Tests.Stubs;

namespace Qluent.NetCore.Tests
{
    [TestFixture]
    public class BuilderTests
    {
        [Test]
        public async Task Given_a_builder_configuration_When_no_storage_account_is_specified_Then_the_builder_should_use_development_storage()
        {
            var q = Builder
                .CreateAQueueOf<Person>()
                .UsingStorageQueue("my-test-queue")
                .Build();

            await q.PurgeAsync();
        }

        [Test]
        public async Task Given_a_builder_configuration_When_a_storage_account_is_specified_Then_the_builder_should_use_that_storage_account()
        {
            var q = Builder
                .CreateAQueueOf<Person>()
                .ConnectedToAccount("UseDevelopmentStorage=true")
                .UsingStorageQueue("my-test-queue")
                .Build();

            await q.PurgeAsync();
        }

        [Test]
        public async Task Given_a_builder_configuration_When_a_queue_is_passed_to_consumer_Then_the_builder_should_return_that_consumer()
        {
            var q = await Builder
                .CreateAQueueOf<Person>()
                .ConnectedToAccount("UseDevelopmentStorage=true")
                .UsingStorageQueue("my-test-queue")
                .BuildAsync();

            var consumer = Builder
                .CreateAMessageConsumerFor<Person>()
                .UsingQueue(q)
                .ThatHandlesMessagesUsing(async (msg, ct) =>
                {
                    Console.WriteLine($"Processing {msg.Value.Name}");
                    return await Task.FromResult(true);
                })
                .Build();

            Assert.IsNotNull(consumer);
        }
    }
}
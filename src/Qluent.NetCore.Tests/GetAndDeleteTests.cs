using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace Qluent.NetCore.Tests
{
    [TestFixture]
    public class GetAndDeleteTests
    {
        [Test]
        public async Task Given_a_simple_queue_When_a_message_is_fetched_using_get_Then_it_should_reappear_after_its_visibilty_timeout()
        {
            var q = Builder
                .CreateAQueueOf<Guid>()
                .UsingStorageQueue("my-test-queue")
                .ThatKeepsMessagesInvisibleAfterDequeuingFor(TimeSpan.FromSeconds(1))
                .Build();

            var guid = Guid.NewGuid();

            await q.PurgeAsync();
            await q.PushAsync(guid);

            var p1 = await q.GetAsync();
            
            await Task.Delay(1200);

            var p2 = await q.GetAsync();
            Assert.AreEqual(p1.Value, p2.Value);
            await q.PurgeAsync();
        }

        [Test]
        public async Task Given_a_simple_queue_When_a_message_is_fetched_using_get_and_deleted_Then_it_should_disappear_from_the_queue()
        {
            var q = Builder
                .CreateAQueueOf<Guid>()
                .UsingStorageQueue("my-test-queue")
                .ThatKeepsMessagesInvisibleAfterDequeuingFor(TimeSpan.FromSeconds(2))
                .Build();

            var guid = Guid.NewGuid();

            await q.PurgeAsync();
            await q.PushAsync(guid);

            var p1 = await q.GetAsync();
            Assert.AreEqual(guid, p1.Value);
            Assert.AreEqual(1, await q.CountAsync());

            await q.DeleteAsync(p1);
            Assert.AreEqual(0, await q.CountAsync());
            
            await q.PurgeAsync();
        }

    }
}

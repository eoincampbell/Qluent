using NUnit.Framework;
using Qluent.NetCore.Tests.Stubs;
using System;
using System.Threading.Tasks;

namespace Qluent.NetCore.Tests
{
    [TestFixture]
    public class MessageVisibilityTests
    {
        [Test]
        public async Task Given_a_queue_with_an_initial_visibility_delay_When_a_message_is_enqueued_Then_it_should_remain_invisible_until_after_the_delay_period()
        {
            var q = Builder
                .CreateAQueueOf<Person>()
                .UsingStorageQueue("my-test-queue")
                .ThatDelaysMessageVisibilityAfterEnqueuingFor(TimeSpan.FromSeconds(1))
                .Build();

            var person = Person.Create();

            await q.PurgeAsync();
            await q.PushAsync(person);

            var p1 = await q.PopAsync();
            Assert.IsNull(p1);
            await Task.Delay(1200);

            var p2 = await q.PopAsync();
            Assert.IsNotNull(p2);
            await q.PurgeAsync();
        }


        [Test]
        public async Task Given_a_queue_with_an_message_visibility_timeout_delay_When_a_message_is_dequeued_Then_it_should_reappear_in_the_queue_invisible_until_after_the_timeout()
        {
            var q = await Builder
                .CreateAQueueOf<Guid>()
                .UsingStorageQueue("my-test-queue")
                .ThatKeepsMessagesInvisibleAfterDequeuingFor(TimeSpan.FromSeconds(1))
                .BuildAsync();

            Guid g = Guid.NewGuid();
            
            await q.PurgeAsync();
            await q.PushAsync(g);

            var message1 = await q.GetAsync();
            await Task.Delay(1200);
            var message2 = await q.GetAsync();

            Assert.AreEqual(message1.Value, message2.Value);
        }

        [Test]
        public async Task Given_a_queue_with_an_message_ttl_When_a_message_is_not_dequeued_before_the_ttl_Then_it_should_disappear_from_the_queue()
        {
            var q = await Builder
                .CreateAQueueOf<Person>()
                .UsingStorageQueue("my-test-queue")
                .ThatSetsAMessageTtlOf(TimeSpan.FromSeconds(1))
                .BuildAsync();

            var person = Person.Create();

            await q.PurgeAsync();
            await q.PushAsync(person);

            var p1 = await q.PeekAsync();
            Assert.IsNotNull(p1);
            await Task.Delay(1200);

            var p2 = await q.PeekAsync();
            Assert.IsNull(p2);
            await q.PurgeAsync();
        }
    }
}

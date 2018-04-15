using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using System.Transactions;
using NUnit.Framework;
using Qluent.NetFramework.Tests.Helpers;
using Qluent.NetFramework.Tests.Stubs;

namespace Qluent.NetFramework.Tests
{
    [TestFixture]
    public class TransactionalAzureStorageQueueTests
    {
        [Test]
        public async Task Given_a_transactionscope_aware_queue_When_a_message_is_pushed_Then_it_should_only_be_added_to_the_queue_after_the_complete_commit_phase()
        {
            var q = Builder
                .CreateAQueueOf<Person>()
                .ConnectedToAccount("UseDevelopmentStorage=true")
                .UsingStorageQueue("my-test-queue")
                .ThatIsTransactionScopeAware()
                .Build();

            var message = Person.Create();
            await q.PurgeAsync();

            using (var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                await q.PushAsync(message);
                Assert.AreEqual(0, await q.CountAsync());
                ts.Complete();
            }

            await Task.Delay(1000);
            Assert.AreEqual(1, await q.CountAsync());
        }

        [Test]
        public async Task Given_a_transactionscope_aware_queue_When_a_message_is_pushed_Then_it_should_not_be_added_to_the_queue_after_the_rollback_phase()
        {
            var q = Builder
                .CreateAQueueOf<Person>()
                .ConnectedToAccount("UseDevelopmentStorage=true")
                .UsingStorageQueue("my-test-queue")
                .ThatIsTransactionScopeAware()
                .Build();

            var message = Person.Create();
            await q.PurgeAsync();

            using (var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                await q.PushAsync(message);
                Assert.AreEqual(0, await q.CountAsync());
            }

            await Task.Delay(1000);
            Assert.AreEqual(0, await q.CountAsync());
        }
    }
}
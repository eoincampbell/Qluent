using System.Threading.Tasks;
using System.Transactions;
using NUnit.Framework;
using Qluent.NetFramework.Tests.Helpers;
using Qluent.NetFramework.Tests.Stubs;

namespace Qluent.NetFramework.Tests
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
    }
}
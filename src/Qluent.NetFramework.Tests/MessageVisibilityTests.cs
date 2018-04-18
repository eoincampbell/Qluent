using NUnit.Framework;
using Qluent.NetFramework.Tests.Stubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qluent.NetFramework.Tests
{
    [TestFixture]
    public class MessageVisibilityTests
    {
        public void Test1()
        {
            var q = Builder
                .CreateAQueueOf<Person>()
                .UsingStorageQueue("my-test-queue")
                .ThatDelaysMessageVisibilityAfterEnqueuingFor(TimeSpan.FromMinutes(1))
                .Build();

            await q.PurgeAsync();
        }
    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Odin.Providers.MemoryStoreProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Odin.Tests
{
    [TestClass]
    public class ConsumerTests
    {
        class Foo
        {
            public string Bar { get; set; }
        }

        [TestMethod]
        public async Task TestJsonConsumer()
        {
            var memoryStore = new OdinMemoryStore();
            var jsonConsumer = new JsonSerializer.OdinJsonSerializer<Foo>(memoryStore);

            var foo = new Foo { Bar = "Baz" };
            await jsonConsumer.Put("foo", foo);

            Assert.IsNotNull(await memoryStore.Get("foo"));
            Assert.AreEqual("Baz", (await jsonConsumer.Get("foo")).Bar);

            Assert.AreEqual("Baz", (await jsonConsumer.Search()).First().Value.Bar);
        }

    }
}

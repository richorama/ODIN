using Microsoft.VisualStudio.TestTools.UnitTesting;
using Odin.Consumers.Triplestore;
using Odin.Providers.MemoryStoreProvider;
using System.Linq;
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

        [TestMethod]
        public async Task TestTripleStore()
        {
            var memStore = new OdinMemoryStore();
            var store = new OdinTriplestore(memStore);
            await store.Put("Richard", "Loves", "Cheese");
            await store.Put("Richard", "Hates", "Marmite");
            await store.Put("Dave", "Loves", "Marmite");

            Assert.AreEqual(9, (await memStore.Search()).Count());
            Assert.AreEqual(1, (await store.Get("Richard", "Loves", "Cheese")).Count());
            Assert.AreEqual(2, (await store.Get(subject: "Richard")).Count());
            Assert.AreEqual(3, (await store.Get()).Count());
        }
    }
}

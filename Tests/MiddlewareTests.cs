using Odin.MemoryStoreProvider;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Odin.Middleware;
using System.Threading.Tasks;

namespace Odin.Tests
{
    [TestClass]
    public class MiddlewareTests
    {
        [TestMethod]
        public async Task TestSubLevel()
        {
            var odin = new OdinMemoryStore();
            var subLevel = new Partition(odin, "foo");
            await OdinTests.BasicOperations(subLevel);
        }

        [TestMethod]
        public async Task TestSubSubLevel()
        {
            var odin = new OdinMemoryStore();
            var fooLevel = new Partition(odin, "foo");

            await fooLevel.Put("x", "x");

            var barLevel = fooLevel.CreateSubPartition("bar");
            await OdinTests.BasicOperations(barLevel);

            await barLevel.Put("y", "y");

            Assert.IsNull(await barLevel.Get("x"));
            Assert.IsNull(await fooLevel.Get("y"));

            await barLevel.Delete("y");
            await fooLevel.Delete("x");
        }

        [TestMethod]
        public async Task TestFanOut()
        {
            var store1 = new OdinMemoryStore();
            var store2 = new OdinMemoryStore();

            var fanOut = new FanOut(store1, store2);
            await OdinTests.BasicOperations(fanOut);

            await fanOut.Put("X", "X");

            Assert.AreEqual("X", await store1.Get("X"));
            Assert.AreEqual("X", await store2.Get("X"));
        }

    }
}

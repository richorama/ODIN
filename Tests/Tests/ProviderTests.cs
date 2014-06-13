using Microsoft.VisualStudio.TestTools.UnitTesting;
using Odin.Providers.FileStoreProvider;
using Odin.Providers.MemoryStoreProvider;
using Odin.Providers.RedisStoreProvider;
using Odin.Providers.TableStoreProvider;
using System.Threading.Tasks;

namespace Odin.Tests
{
    [TestClass]
    public class ProviderTests
    {
        [TestMethod]
        public async Task TableStore()
        {
            var tableStore = new OdinTableStore("table1", "partition1");
            tableStore.CreateTable().Wait();
            await OdinTests.BasicOperations(tableStore);
        }

        [TestMethod]
        public async Task MemoryStore()
        {
            var tableStore = new OdinMemoryStore();
            await OdinTests.BasicOperations(tableStore);
        }

        [TestMethod]
        public async Task FileStore()
        {
            var fileStore = new OdinFileStore(@"c:\test");
            await OdinTests.BasicOperations(fileStore);
        }


        [TestMethod]
        public async Task RedisStore()
        {
            var redisStore = new OdinRedisStore();
            await OdinTests.BasicOperations(redisStore);
        }

    }
}

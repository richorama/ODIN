using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Odin.Providers.TableStoreProvider;
using Odin.Providers.MemoryStoreProvider;
using Odin.Providers.FileStoreProvider;

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

    }
}

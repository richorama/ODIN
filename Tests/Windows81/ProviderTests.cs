using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System.Threading.Tasks;
using Odin.Providers.Windows81Provider;

namespace Odin.Tests.Windows81
{
    [TestClass]
    public class Windows81ProviderTests
    {
        [TestMethod]
        public async Task Windows81SettingsStore()
        {
            var settingsStore = new OdinSettingsStore();
            await BasicOperations(settingsStore);
        }

        [TestMethod]
        public async Task Windows81FileStore()
        {
            var settingsStore = new OdinFileStore();
            await BasicOperations(settingsStore);
        }


        public static async Task BasicOperations(IOdin clive)
        {
            await clive.Put("foo", "bar");
            Assert.AreEqual("bar", await clive.Get("foo"));
            await clive.Put("foo", "baz");
            Assert.AreEqual("baz", await clive.Get("foo"));
            await clive.Delete("foo");
            Assert.AreEqual(null, await clive.Get("foo"));
            await clive.Delete("foo");

            var tasks = new List<Task>();
            for (var i = 0; i < 100; i++)
            {
                tasks.Add(clive.Put(i.ToString().PadLeft(4, '0'), i.ToString()));
            }
            await Task.WhenAll(tasks);
            tasks.Clear();

            var items = (await clive.Search()).ToArray();
            Assert.AreEqual(100, items.Length);
            Assert.AreEqual("0000", items[0].Key);
            Assert.AreEqual("0099", items[99].Key);

            items = (await clive.Search(start: "0001", end: "0010")).ToArray();
            Assert.AreEqual(10, items.Length);
            Assert.AreEqual("0001", items[0].Key);
            Assert.AreEqual("0010", items[9].Key);

            for (var i = 0; i < 100; i++)
            {
                tasks.Add(clive.Delete(i.ToString()));
            }
        }

    }
}

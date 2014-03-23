using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Odin.Middleware.Versioner
{
    public class OdinVersioner : IOdin
    {
        public IOdin Target { get; private set; }

        readonly IOdin master;

        public OdinVersioner(IOdin target)
        {
            this.Target = target;
            this.master = new Partition(target, "__MASTER_KEYS");
        }

        public async Task Put(string key, string value)
        {
            var now = DateTime.UtcNow;
            var versionLevel = new Partition(this.Target, key);
            await Task.WhenAll(
                versionLevel.Put((now.Ticks).ToString("d19"), value),
                this.master.Put(key, value));
        }

        public Task<string> Get(string key)
        {
            return this.master.Get(key);
        }

        public Task Delete(string key)
        {
            return this.master.Delete(key);
        }

        public Task<IEnumerable<KeyValue>> Search(string start = null, string end = null)
        {
            return this.master.Search(start, end);
        }

        public Task<IEnumerable<KeyValue>> GetVersions(string key)
        {
            return new Partition(this.Target, key).Search();
        }
    }
}

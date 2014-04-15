using System;
using System.Collections.Generic;
using System.Runtime.Caching;
using System.Threading.Tasks;

namespace Odin.Middleware
{
    public class OdinCache : IOdin
    {
        public IOdin Store { get; private set; }
        public MemoryCache Cache { get; private set; }

        public TimeSpan Duration { get; private set; }

        public OdinCache(IOdin store, TimeSpan duration)
        {
            this.Store = store;
            this.Cache = new MemoryCache("odin");
        }

        public async Task Put(string key, string value)
        {
            if (this.Cache.Contains(key) && this.Cache[key] as string == value) return;
            this.Cache.Add(key, value, new DateTimeOffset(DateTime.UtcNow, this.Duration));
            await this.Store.Put(key, value);
        }

        public async Task<string> Get(string key)
        {
            var result = this.Cache[key] as string;
            if (result == null) return await this.Store.Get(key);
            return result;
        }

        public async Task Delete(string key)
        {
            this.Cache.Remove(key);
            await this.Store.Delete(key);
        }

        public Task<IEnumerable<KeyValue>> Search(string start = null, string end = null)
        {
            // we can't use the cache for searching
            return this.Store.Search(start, end);
        }
    }

}

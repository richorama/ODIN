using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Odin.Providers.MemoryStoreProvider
{
    public class OdinMemoryStore : IOdin
    {
        ConcurrentDictionary<string, string> dictionary;

        public OdinMemoryStore()
        {
            dictionary = new ConcurrentDictionary<string, string>();
        }

        public async Task Put(string key, string value)
        {
            dictionary.AddOrUpdate(key, value, (a, b) => value);
        }

        public Task<string> Get(string key)
        {
            string value = null;
            dictionary.TryGetValue(key, out value);
            return Task.FromResult<string>(value);
        }

        public async Task Delete(string key)
        {
            string value = null;
            dictionary.TryRemove(key, out value);
        }

        public Task<IEnumerable<KeyValue>> Search(string start = null, string end = null)
        {
             var results = dictionary.OrderBy(x => x.Key);
            if (!string.IsNullOrWhiteSpace(start)) results = results.Where(x => string.Compare(x.Key, start) >= 0).OrderBy(x => x.Key);
            if (!string.IsNullOrWhiteSpace(end)) results = results.Where(x => string.Compare(x.Key, end) <= 0).OrderBy(x => x.Key);
            return Task.FromResult(results.Select(x => new KeyValue { Key = x.Key, Value = x.Value }));
        }

    }
}

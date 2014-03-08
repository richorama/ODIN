using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Odin.Providers.RedisStoreProvider
{
    public class OdinRedisStore : IOdin
    {
        RedisClient client;

        public OdinRedisStore(string host = "localhost", int port = 6379)
        {
            client = new RedisClient(host, port);
        }

        public async Task Put(string key, string value)
        {
            client.Set<string>(key, value);
        }

        public Task<string> Get(string key)
        {
            return Task.FromResult<string>(client.Get<string>(key));
        }

        public async Task Delete(string key)
        {
            client.Del(key);
        }

        public Task<IEnumerable<KeyValue>> Search(string start = null, string end = null)
        {
            // this is really horrible, and needs to be switched for something much better.

            var keys = client.GetAllKeys().OrderBy(x => x);
            if (!string.IsNullOrWhiteSpace(start)) keys = keys.Where(x => string.Compare(x, start) >= 0).OrderBy(x => x);
            if (!string.IsNullOrWhiteSpace(end)) keys = keys.Where(x => string.Compare(x, end) <= 0).OrderBy(x => x);

            return Task.FromResult(client.GetAll<string>(keys).OrderBy(x => x.Key).Select(x => new KeyValue { Key = x.Key, Value = x.Value }));
        }
    }
}

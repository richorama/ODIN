using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Odin.JsonSerializer
{
    public class OdinJsonSerializer<T>
    {
        public IOdin Odin { get; set; }

        public OdinJsonSerializer(IOdin odin)
        {
            this.Odin = odin;
        }

        public Task Put(string key, T value)
        {
            if (value == null)
            {
                return this.Odin.Put(key, null);
            }
            return this.Odin.Put(key, JsonConvert.SerializeObject(value));
        }

        public async Task<T> Get(string key)
        {
            return Convert(await this.Odin.Get(key));
        }

        public Task Delete(string key)
        {
            return this.Odin.Delete(key);
        }

        public async Task<IEnumerable<KeyValueT<T>>> Search(string start = null, string end = null)
        {
            return (await this.Odin.Search(start, end)).Select(x => Convert(x));
        }

        T Convert(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return default(T);
            return JsonConvert.DeserializeObject<T>(value);
        }

        KeyValueT<T> Convert(KeyValue kv)
        { 
            return new KeyValueT<T>{ Key = kv.Key, Value = Convert(kv.Value)};
        }


    }
}

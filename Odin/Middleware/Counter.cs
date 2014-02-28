using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Odin.Middleware
{
    public class Counter : IOdin
    {
        public IOdin Target { get; private set; }

        public int PutCount;
        public int GetCount;
        public int DeleteCount;
        public int SearchCount;

        public Counter(IOdin target)
        {
            this.Target = target;
        }

        public Task Put(string key, string value)
        {
            Interlocked.Increment(ref this.PutCount);
            return this.Target.Put(key, value);
        }

        public Task<string> Get(string key)
        {
            Interlocked.Increment(ref this.GetCount);
            return this.Target.Get(key);
        }


        public Task Delete(string key)
        {
            Interlocked.Increment(ref this.DeleteCount);
            return this.Target.Delete(key);
        }

        public Task<IEnumerable<KeyValue>> Search(string start = null, string end = null)
        {
            Interlocked.Increment(ref this.SearchCount);
            return this.Target.Search(start, end);
        }


    }
}

using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Odin.Middleware
{
    public class OdinTracer : IOdin
    {
        public IOdin Target { get; private set; }

        readonly bool traceGet;
        readonly bool tracePut;
        readonly bool traceDelete;
        readonly bool traceSearch;

        public OdinTracer(IOdin target, bool traceGet = true, bool tracePut = true, bool traceDelete = true, bool traceSearch = true)
        {
            this.traceGet = traceGet;
            this.tracePut = tracePut;
            this.traceDelete = traceDelete;
            this.traceSearch = traceSearch;
            this.Target = target;
        }

        public Task Put(string key, string value)
        {
            if (this.tracePut) Trace.WriteLine(string.Format("PUT:{0},{1}", key, value));
            return this.Target.Put(key, value);
        }

        public Task<string> Get(string key)
        {
            if (this.traceGet) Trace.WriteLine(string.Format("GET:{0}", key));
            return this.Target.Get(key);
        }


        public Task Delete(string key)
        {
            if (this.traceDelete) Trace.WriteLine(string.Format("DELETE:{0}", key));
            return this.Target.Delete(key);
        }

        public Task<IEnumerable<KeyValue>> Search(string start = null, string end = null)
        {
            if (this.traceSearch) Trace.WriteLine(string.Format("SEARCH:start={0},end={1}", start, end));
            return this.Target.Search(start, end);
        }


    }
}

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Odin.Middleware
{

    public class LoadBalancer : IOdin
    {
        public enum Strategy
        {
            RoundRobin
        }

        public Strategy CurrentStrategy { get; private set; }

        public IOdin[] Stores { get; private set; }

        int writeCount = 0;

        public LoadBalancer(LoadBalancer.Strategy strategy, params IOdin[] stores)
        {
            this.CurrentStrategy = strategy;
            this.Stores = stores;
        }

        public Task Put(string key, string value)
        {
            return this.SelectStore(key).Put(key, value);
        }

        public Task<string> Get(string key)
        {
            return this.SelectStore(key).Get(key);
        }

        public Task Delete(string key)
        {
            return this.SelectStore(key).Delete(key);
        }

        public Task<IEnumerable<KeyValue>> Search(string start = null, string end = null)
        {
            return this.SelectStore(null).Search(start, end);
        }


        IOdin SelectStore(string key)
        {
            switch (this.CurrentStrategy)
            {
                case Strategy.RoundRobin:
                    return this.Stores[Interlocked.Increment(ref writeCount) % this.Stores.Length];
                default:
                    throw new NotImplementedException();
            }
        }
    }
}

using Odin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Odin.Middleware
{
    public class FanOut : IOdin
    {
        public IOdin[] Reads { get; private set; }
        public IOdin[] Writes { get; private set; }

        public FanOut(params IOdin[] odins)
        {
            this.Reads = odins;
            this.Writes = odins;
        }

        public FanOut(IOdin[] reads, IOdin[] writes)
        {
            this.Reads = reads;
            this.Writes = writes;
        }
        public async Task Put(string key, string value)
        {
            var tasks = new List<Task>();
            foreach (var writer in this.Writes)
            {
                tasks.Add(writer.Put(key, value));
            }
            await Task.WhenAll(tasks);
        }

        public async Task<string> Get(string key)
        {
            var tasks = new List<Task<string>>();
            foreach (var reader in this.Reads)
            {
                tasks.Add(reader.Get(key));
            }
            await Task.WhenAny<string>(tasks);
            return tasks.First(x => x.IsCompleted).Result;
        }

        public async Task Delete(string key)
        {
            var tasks = new List<Task>();
            foreach (var writer in this.Writes)
            {
                tasks.Add(writer.Delete(key));
            }
            await Task.WhenAll(tasks);
        }

        public async Task<IEnumerable<KeyValue>> Search(string start = null, string end = null)
        {
            var tasks = new List<Task<IEnumerable<KeyValue>>>();
            foreach (var reader in this.Reads)
            {
                tasks.Add(reader.Search(start, end));
            }
            await Task.WhenAny<IEnumerable<KeyValue>>(tasks);
            return tasks.First(x => x.IsCompleted).Result;
        }
    }
}

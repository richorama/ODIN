using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Odin.Middleware
{
    public class Partition : IOdin
    {
        IOdin odin;
        public string PartitionName { get; private set; }

        public string Seperator { get; private set; }

        public Partition(IOdin odin, string partition, char seperator = (char)255)
        {
            this.odin = odin;
            this.PartitionName = partition;
            this.Seperator = "" + seperator;
        }

        public IOdin CreateSubPartition(string nextLevel)
        {
            return new Partition(this, this.PartitionName + nextLevel);
        }

        public async Task Put(string key, string value)
        {
            await this.odin.Put(ApplyPartition(key), value);
        }

        public async Task<string> Get(string key)
        {
            return await this.odin.Get(ApplyPartition(key));
        }

        public async Task Delete(string key)
        {
            await this.odin.Delete(ApplyPartition(key));
        }

        public async Task<IEnumerable<KeyValue>> Search(string start = null, string end = null)
        {

            if (string.IsNullOrEmpty(start)) start = "";
            if (string.IsNullOrEmpty(end)) end = "";

            start = ApplyPartition(start);
            end = ApplyPartition(end) + this.Seperator;

            return (await this.odin.Search(start, end)).Select(x => { x.Key = RemovePartition(x.Key); return x; });
        }

        private string ApplyPartition(string key)
        {
            return string.Format("{2}{0}{2}{1}", this.PartitionName, key, this.Seperator);
        }

        private string RemovePartition(string key)
        {
            return key.Replace(this.Seperator + this.PartitionName + this.Seperator, "");
        }

    }
}

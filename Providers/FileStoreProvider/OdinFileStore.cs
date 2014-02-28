using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Odin.FileStoreProvider
{
    public class OdinFileStore : IOdin
    {
        public string Directory { get; set; }

        public OdinFileStore(string directory = ".")
        {
            this.Directory = directory;
            if (!System.IO.Directory.Exists(this.Directory))
            {
                System.IO.Directory.CreateDirectory(this.Directory);
            }
        }

        public async Task Put(string key, string value)
        {
            File.WriteAllText(Path.Combine(this.Directory, key), value);
        }

        public Task<string> Get(string key)
        {
            try
            {
                return Task.FromResult<string>(File.ReadAllText(Path.Combine(this.Directory, key)));
            }
            catch (FileNotFoundException)
            {
                return Task.FromResult<string>(null);
            }
        }

        public async Task Delete(string key)
        {
            try
            {
                File.Delete(Path.Combine(this.Directory, key));
            }
            catch (FileNotFoundException)
            { }
        }

        public Task<IEnumerable<KeyValue>> Search(string start = null, string end = null)
        {
            var results = System.IO.Directory.GetFiles(this.Directory).OrderBy(x => x); ;
            if (!string.IsNullOrWhiteSpace(start)) results = results.Where(x => string.Compare(Path.GetFileName(x), start) >= 0).OrderBy(x => x);
            if (!string.IsNullOrWhiteSpace(end)) results = results.Where(x => string.Compare(Path.GetFileName(x), end) <= 0).OrderBy(x => x);
            return Task.FromResult(results.Select(x => new KeyValue { Key = Path.GetFileName(x), Value = this.Get(x).Result }));

        }
    }
}

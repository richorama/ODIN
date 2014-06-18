using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;

namespace Odin.Providers.Windows81Provider
{
    public class OdinFileStore : IOdin
    {
        public OdinFileStore(StorageFolder folder)
        {
            this.Folder = folder;
        }

        public OdinFileStore()
        {
            this.Folder = ApplicationData.Current.LocalFolder;
        }

        public static OdinFileStore CreateLocalFolder()
        {
            return new OdinFileStore(ApplicationData.Current.LocalFolder);
        }

        public static OdinFileStore CreateRoamingFolder()
        {
            return new OdinFileStore(ApplicationData.Current.RoamingFolder);
        }

        public StorageFolder Folder { get; private set; }

        public async Task Put(string key, string value)
        {
            var file = await this.Folder.CreateFileAsync(key, CreationCollisionOption.ReplaceExisting).AsTask();
            await FileIO.WriteTextAsync(file, value).AsTask();
        }

        public async Task<string> Get(string key)
        {
            try
            {
                var file = await ApplicationData.Current.LocalFolder.GetFileAsync(key).AsTask();
                return await FileIO.ReadTextAsync(file).AsTask();
            }
            catch
            {
                return null;
            }
        }

        public async Task Delete(string key)
        {
            var file = await ApplicationData.Current.LocalFolder.CreateFileAsync(key, CreationCollisionOption.ReplaceExisting).AsTask();
            await file.DeleteAsync().AsTask();
        }

        public async Task<IEnumerable<KeyValue>> Search(string start = null, string end = null)
        {
            var files = await this.Folder.GetFilesAsync();

            var results = files.OrderBy(x => x.Name);
            if (!string.IsNullOrWhiteSpace(start)) results = results.Where(x => string.Compare(x.Name, start) >= 0).OrderBy(x => x.Name);
            if (!string.IsNullOrWhiteSpace(end)) results = results.Where(x => string.Compare(x.Name, end) <= 0).OrderBy(x => x.Name);
            var output = new List<KeyValue>();
            foreach (var item in results)
            {
                output.Add(new KeyValue { Key = item.Name, Value = await this.Get(item.Name) });
            }
            return output;
        }
    }
}

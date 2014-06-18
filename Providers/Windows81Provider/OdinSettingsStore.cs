using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;

namespace Odin.Providers.Windows81Provider
{
    public class OdinSettingsStore : IOdin
    {
        public ApplicationDataContainer Container { get; private set; }

        public OdinSettingsStore(ApplicationDataContainer container)
        {
            if (null == container) throw new ArgumentNullException("container");
            this.Container = container;
        }

        public OdinSettingsStore()
        {
            this.Container = Windows.Storage.ApplicationData.Current.LocalSettings;
        }

        public OdinSettingsStore CreateForLocalSettings()
        {
            return new OdinSettingsStore(Windows.Storage.ApplicationData.Current.LocalSettings);
        }

        public OdinSettingsStore CreateForRoamingSettings()
        {
            return new OdinSettingsStore(Windows.Storage.ApplicationData.Current.RoamingSettings);
        }


        public async Task Put(string key, string value)
        {
            this.Container.Values[key] = value;
        }

        public async Task<string> Get(string key)
        {
            if (this.Container.Values.ContainsKey(key))
            {
                return this.Container.Values[key] as string;
            }
            return null;
        }

        public async Task Delete(string key)
        {
            this.Container.Values.Remove(key);
        }

        public Task<IEnumerable<KeyValue>> Search(string start = null, string end = null)
        {
            var results = this.Container.Values.OrderBy(x => x.Key);
            if (!string.IsNullOrWhiteSpace(start)) results = results.Where(x => string.Compare(x.Key, start) >= 0).OrderBy(x => x.Key);
            if (!string.IsNullOrWhiteSpace(end)) results = results.Where(x => string.Compare(x.Key, end) <= 0).OrderBy(x => x.Key);
            return Task.FromResult(results.Select(x => new KeyValue { Key = x.Key, Value = x.Value as string }));
        }
    }
}

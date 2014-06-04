using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Odin.Providers.WindowsPhoneProvider
{
    public class OdinSettingsStore : IOdin
    {
        public async Task Put(string key, string value)
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            localSettings.Values[key] = value;
        }

        public async Task<string> Get(string key)
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            if (localSettings.Values.ContainsKey(key))
            {
                return localSettings.Values[key] as string;
            }
            return null;
        }

        public async Task Delete(string key)
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            localSettings.Values.Remove(key);
        }

        public Task<IEnumerable<KeyValue>> Search(string start = null, string end = null)
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            var results = localSettings.Values.OrderBy(x => x.Key);
            if (!string.IsNullOrWhiteSpace(start)) results = results.Where(x => string.Compare(x.Key, start) >= 0).OrderBy(x => x.Key);
            if (!string.IsNullOrWhiteSpace(end)) results = results.Where(x => string.Compare(x.Key, end) <= 0).OrderBy(x => x.Key);
            return Task.FromResult(results.Select(x => new KeyValue { Key = x.Key, Value = x.Value as string }));
        }
    }
}

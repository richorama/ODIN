using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Odin
{
    public interface IOdin
    {
        Task Put(string key, string value);
        
        Task<string> Get(string key);

        Task Delete(string key);

        Task<IEnumerable<KeyValue>> Search(string start = null, string end = null);
    }


}

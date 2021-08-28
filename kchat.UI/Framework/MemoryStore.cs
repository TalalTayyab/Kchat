using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kchat.UI.Framework
{
    public class MemoryStore
    {
        public ConcurrentDictionary<string, string> _keyValuePairs = new ConcurrentDictionary<string, string>();

        public void Add(string key, string value)
        {
            _keyValuePairs.AddOrUpdate(key, value, (key, oldvalue) => value);
        }

        public string Get(string key)
        {
            return _keyValuePairs.GetValueOrDefault(key);
        }
    }
}

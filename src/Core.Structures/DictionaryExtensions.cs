using System;
using System.Collections.Generic;

namespace Core.Structures
{
    public static class DictionaryExtensions
    {
        public static TValue GetOrCreate<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, Func<TValue> factory)
        {
            return dict.TryGetValue(key, out TValue result) ? result : dict[key] = factory();
        }
    }
}
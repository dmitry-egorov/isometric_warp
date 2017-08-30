using System;
using System.Collections.Generic;

namespace Lanski.Structures
{
    public static class DictionarySemanticExtensions
    {
        public static MustHave<TKey, TValue> Must_Have<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key) => new MustHave<TKey, TValue>(dict, key);

        public struct MustHave<TKey, TValue>
        {
            private readonly Dictionary<TKey, TValue> _dict;
            private readonly TKey _key;

            public MustHave(Dictionary<TKey, TValue> dict, TKey key)
            {
                _dict = dict;
                _key = key;
            }

            public TValue Otherwise(Exception exception) => _dict.has_the(_key).@as(out var result) ? result : throw exception;
        }

        public static ContainsThe<TKey, TValue> has_the<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key) => new ContainsThe<TKey, TValue>(dict, key); 

        public struct ContainsThe<TKey, TValue>
        {
            private readonly Dictionary<TKey, TValue> _dict;
            private readonly TKey _key;

            public ContainsThe(Dictionary<TKey, TValue> dict, TKey key)
            {
                _dict = dict;
                _key = key;
            }
            
            public bool @as(out TValue value) => _dict.TryGetValue(_key, out value);
        }
    }
}
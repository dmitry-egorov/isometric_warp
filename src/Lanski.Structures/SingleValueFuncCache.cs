using System;

namespace Lanski.Structures
{
    public struct SingleValueFuncCache<TSource, TValue>
    {
        private readonly Func<TSource, TValue> _func;
        
        private Cache? _cache;

        public SingleValueFuncCache(Func<TSource, TValue> func)
        {
            _func = func;
            _cache = null;
        }

        public TValue Get(TSource source)
        {
            if (!_cache.Has_a_Value(out var last_calc) || !last_calc.Source.Equals(source))
                _cache = last_calc = new Cache(source, _func(source));

            return last_calc.Result;
        }

        private struct Cache
        {
            public readonly TSource Source;
            public readonly TValue Result;

            public Cache(TSource source, TValue result)
            {
                Source = source;
                Result = result;
            }
        }
    }
}
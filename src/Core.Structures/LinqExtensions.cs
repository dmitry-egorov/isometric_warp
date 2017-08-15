using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Core.Structures
{
    using System;

    public static class LinqExtensions
    {
        public static ReadOnlyCollection<T> ToReadOnlyCollection<T>(this IEnumerable<T> enumerable)
        {
            return new ReadOnlyCollection<T>(enumerable.ToArray());
        }

        public static void Do<T>(this IEnumerable<T> e, Action<T> action)
        {
            foreach (var item in e)
            {
                action(item);
            }
        }

        public static IEnumerable<float> RunningSum(this IEnumerable<float> input)
        {
            return input.Scan((r, e) => r + e);
        }

        public static IEnumerable<U> Scan<T, U>(this IEnumerable<T> input, Func<U, T, U> next, U state)
        {
            yield return state;
            foreach (var item in input)
            {
                state = next(state, item);
                yield return state;
            }
        }

        public static IEnumerable<T> Scan<T>(this IEnumerable<T> input, Func<T, T, T> next)
        {
            using (var e = input.GetEnumerator())
            {
                if(!e.MoveNext())
                    throw new InvalidOperationException("input is empty");

                var state = e.Current;
                yield return state;

                while (e.MoveNext())
                {
                    state = next(state, e.Current);
                    yield return state;
                }
            }
        }
    }
}
using System;
using System.Collections.Generic;

namespace Lanski.SwiftLinq
{
    public static class EnumerateExtensions
    {
        public static void SForEach<T>(this IReadOnlyList<T> list, Action<T> the_action)
        {
            var the_iterator = list.s_New_Iterator();
            while (the_iterator.has_a_Value(out var the_value))
            {
                the_action(the_value);
            }
        }

        public static SIterator<T> s_New_Iterator<T>(this IReadOnlyList<T> list) => new SIterator<T>(list);

        public struct SIterator<T>
        {
            private readonly IReadOnlyList<T> its_list;
            private int its_index;

            public SIterator(IReadOnlyList<T> the_list)
            {
                its_list = the_list;
                its_index = 0;
            }

            public bool has_a_Value(out T value)
            {
                if (its_index >= its_list.Count)
                {
                    value = default(T);
                    return false;
                }

                value = its_list[its_index];
                its_index++;
                return true;
            }
            
        }
    }
}
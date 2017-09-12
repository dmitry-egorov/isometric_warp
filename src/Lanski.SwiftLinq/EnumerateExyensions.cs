using System.Collections.Generic;

namespace Lanski.SwiftLinq
{
    public static class EnumerateExyensions
    {
        public static SIterator<T> s_new_iterator<T>(this IReadOnlyList<T> list) => new SIterator<T>(list);

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
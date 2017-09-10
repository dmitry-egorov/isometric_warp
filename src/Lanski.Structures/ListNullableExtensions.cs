using System.Collections.Generic;

namespace Lanski.Structures
{
    public static class ListNullableExtensions
    {
        public static bool has_a_Value_At<T>(this IReadOnlyList<T> the_list, int the_index, out T value) =>
            the_list.s_possible_Value_At(the_index).has_a_Value(out value)
        ; 
        public static Possible<T> s_possible_Value_At<T>(this IReadOnlyList<T> the_list, int the_index) => 
            the_index >= 0 && the_index < the_list.Count 
                ? the_list[the_index] 
                : Possible.Empty<T>()
        ;
    }
}
using System.Collections.Generic;

namespace Lanski.Structures
{
    public static class ListNullableExtensions
    {
        public static Possible<T> possible_Element_At<T>(this IReadOnlyList<T> the_list, int the_index) => 
            the_index >= 0 && the_index < the_list.Count 
                ? the_list[the_index] 
                : Possible.Empty<T>()
        ;
    }
}
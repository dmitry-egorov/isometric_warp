using System.Collections.Generic;
using System.Linq;

namespace Lanski.Structures
{
    public static class IntSemantics
    {
        public static Possible<int> as_a_possible_positive(this int the_value) => the_value > 0 ? the_value : Possible.Empty<int>();
        
        /// <summary>
        /// A enumerable of values from 0 to the_count 
        /// </summary>
        public static IEnumerable<int> counted(this int the_count) => Enumerable.Range(0, the_count);
    }
}
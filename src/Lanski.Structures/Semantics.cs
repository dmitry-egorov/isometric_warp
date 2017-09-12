using System;

namespace Lanski.Structures
{
    public static class Semantics
    {
        /// <summary>
        /// Used for assigning initial value to an out parameter in a conditional statement.  
        /// </summary>
        public static bool semantic_resets<T>(out T v)
        {
            v = default(T);
            return true;
        }
        /// <summary>
        /// Used for assigning initial value to an out parameter in a conditional statement.  
        /// </summary>
        public static bool semantic_resets<T1, T2>(out T1 v1, out T2 v2)
        {
            v1 = default(T1);
            v2 = default(T2);
            return true;
        }

        public static bool @is<T>(this T value, T other) where T : IEquatable<T> => value.Equals(other);
        public static bool is_not<T>(this T value, T other) where T : IEquatable<T> => !value.Equals(other);
    }
}
using System;

namespace Lanski.Structures
{
    public static class Flow
    {
        /// <summary>
        /// Used for assigning initial value to an out parameter in a conditional statement.  
        /// </summary>
        public static bool default_as<T>(out T v)
        {
            v = default(T);
            return true;
        }
        /// <summary>
        /// Used for assigning initial value to an out parameter in a conditional statement.  
        /// </summary>
        public static bool default_as<T1, T2>(out T1 v1, out T2 v2)
        {
            v1 = default(T1);
            v2 = default(T2);
            return true;
        }

        public static bool @as<T>(this T value, out T other) { other = value; return true; }

        public static bool @is<T>(this T value, T other) where T : IEquatable<T> => value.Equals(other);
        public static bool is_not<T>(this T value, T other) where T : IEquatable<T> => !value.Equals(other);
    }
}
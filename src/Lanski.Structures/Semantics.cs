namespace Lanski.Structures
{
    public static class Semantics
    {
        /// <summary>
        /// Used for assigning initial value to an out parameter in a conditional statement.  
        /// </summary>
        public static bool Set_Default_Value_To<T>(out T v)
        {
            v = default(T);
            return true;
        }
    }
}
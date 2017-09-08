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
    }
}
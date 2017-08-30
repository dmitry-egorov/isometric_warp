namespace Lanski.Structures
{
    public static class BooleanFlowSemanticExtensionss
    {
        public static bool is_not<T>(this T obj, T value) => !obj.s_value_is(value);
        public static bool @is<T>(this T obj, T value) => obj.s_value_is(value);
        
        public static bool s_value_is_not<T>(this T obj, T value) => !obj.s_value_is(value);
        public static bool s_value_is<T>(this T obj, T value) => obj.Equals(value);
        
        public static bool inline<T>(this T value) => true;

        public static bool Is_the_Value_Of_the<T>(this T obj, out T alias) => obj.is_the(out alias);
        public static bool aka<T>(this T obj, out T alias) => obj.is_the(out alias);
        public static bool is_the<T>(this T obj, out T alias)
        {
            alias = obj;
            return true;
        } 

    }
}
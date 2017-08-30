using Lanski.Structures;

namespace Lanski.Reactive
{
    public static class NullableCellExtensions
    {
        public static bool s_value_is_the<T>(this ICell<T> cell, out T value) where T : class => (value = cell.Value).inline();
        public static bool has_something<T>(this ICell<Slot<T>> cell) where T : class => cell.Value.has_something();
        public static bool has_a_value<T>(this ICell<Slot<T>> cell, out T obj) where T : class => cell.Value.Has_a_Value(out obj);
        
        public static bool has_something<T>(this ICell<T?> cell) where T : struct => cell.Value.HasValue;
        public static bool doesnt_have_a<T>(this ICell<T?> cell, out T value) where T : struct => !cell.has_a_value(out value);
        public static bool has_a_value<T>(this ICell<T?> cell, out T value) where T : struct
        {
            var result = cell.has_something();
            value = result ? cell.Value.Value : default(T);
            return result;
        }
    }
}
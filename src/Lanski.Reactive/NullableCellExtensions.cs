using Lanski.Structures;

namespace Lanski.Reactive
{
    public static class NullableCellExtensions
    {
        public static bool Does_Not_Have_a_Value<T>(this ICell<Possible<T>> cell) where T : class => !cell.Has_a_Value();
        public static bool Has_a_Value<T>(this ICell<Possible<T>> cell) where T : class => cell.Value.Has_a_Value();
        public static bool Has_a_Value<T>(this ICell<Possible<T>> cell, out T obj) where T : class => cell.Value.Has_a_Value(out obj);
        
        public static bool Does_Not_Have_a_Value<T>(this ICell<T?> cell) where T : struct => !cell.Has_a_Value();
        public static bool Does_Not_Have_a_Value<T>(this ICell<T?> cell, out T value) where T : struct => !cell.Has_a_Value(out value);
        public static bool Has_a_Value<T>(this ICell<T?> cell) where T : struct => cell.Value.HasValue;
        public static bool Has_a_Value<T>(this ICell<T?> cell, out T value) where T : struct
        {
            var result = cell.Has_a_Value();
            value = result ? cell.Value.Value : default(T);
            return result;
        }

        public static ICell<T> Cell_Or_Single_Default_Ref<T>(this Possible<ICell<T>> possible_cell) where T : class => possible_cell.Value_Or(new RefCell<T>(default(T)));
        public static ICell<T> Cell_Or_Single_Default<T>(this Possible<ICell<T>> possible_cell) where T : struct => possible_cell.Value_Or(new ValueCell<T>(default(T)));
        public static ICell<T?> Cell_Or_Single_Default_Nullable<T>(this Possible<ICell<T?>> possible_cell) where T : struct => possible_cell.Value_Or(new NullableCell<T>(default(T?)));
        public static IStream<T> Cell_Or_Empty<T>(this Possible<ICell<T>> possible_cell) where T : struct => possible_cell.Select(c => (IStream<T>)c).Value_Or(Stream.Empty<T>());
    }
}
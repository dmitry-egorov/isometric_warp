using Lanski.Structures;

namespace Lanski.Reactive
{
    public static class NullableCellExtensions
    {
        public static T Must_Have_a_Value<T>(this ICell<Possible<T>> cell) => cell.s_Value.Must_Have_a_Value();
        public static bool Has_a_Value<T>(this ICell<Possible<T>> cell, out T obj) => cell.s_Value.Has_a_Value(out obj);
        
        public static ICell<T> Cell_Or_Single_Default<T>(this Possible<ICell<T>> possible_cell) where T : struct => possible_cell.Value_Or(new ValueCell<T>(default(T)));
        public static IStream<T> Cell_Or_Empty<T>(this Possible<ICell<T>> possible_cell) where T : struct => possible_cell.Select(c => (IStream<T>)c).Value_Or(Stream.Empty<T>());
    }
}
using System;
using Lanski.Structures;

namespace Lanski.Reactive
{
    public static class NullableCellExtensions
    {
        public static T must_have_a_Value<T>(this ICell<Possible<T>> cell) => cell.s_Value.must_have_a_Value();
        public static bool has_a_Value<T>(this ICell<Possible<T>> cell, out T obj) => cell.s_Value.has_a_Value(out obj);
        
        public static ICell<TOut> Select_Cell_Or_Single_Default<TIn, TOut>(this Possible<TIn> possible, Func<TIn, ICell<TOut>> selector) => possible.Select(selector).Value_Or(CellsCache<TOut>.Default);
        
        public static ICell<T> Cell_Or_Single_Default<T>(this Possible<ICell<T>> possible_cell) where T : struct => possible_cell.Value_Or(CellsCache<T>.Default);
        public static IStream<T> Cell_Or_Empty<T>(this Possible<ICell<T>> possible_cell) where T : struct => possible_cell.Select(c => (IStream<T>)c).Value_Or(StreamsCache<T>.Empty);
    }
}
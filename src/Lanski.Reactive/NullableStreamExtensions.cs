﻿using System;
using Lanski.Structures;

namespace Lanski.Reactive
{
    internal static class CellsCache<T>
    {
        public static readonly ICell<T> Default = new Cell<T>(default(T));
    }
    
    internal static class StreamsCache<T>
    {
        public static readonly IStream<T> Empty = Stream.Empty<T>();
    }
    
    public static class NullableStreamExtensions
    {
        public static IStream<TOut> Select_Stream_Or_Empty<TIn, TOut>(this Possible<TIn> possible, Func<TIn, IStream<TOut>> selector) => possible.Select(selector).s_Value_Or(StreamsCache<TOut>.Empty);
        public static IStream<TOut> Select_Stream_Or_Single_Default<TIn, TOut>(this Possible<TIn> possible, Func<TIn, IStream<TOut>> selector) => possible.Select(selector).s_Value_Or(CellsCache<TOut>.Default);
        public static IStream<T> Value_Or_Empty<T>(this Possible<IStream<T>> possible_stream) => possible_stream.Select(c => c).s_Value_Or(StreamsCache<T>.Empty);
        public static IStream<TOut> SelectMany_Or_Single_Default<TIn, TOut>(this IStream<Possible<TIn>> stream, Func<TIn, IStream<TOut>> selector) => stream.SelectMany(p => p.Select(selector).s_Value_Or(CellsCache<TOut>.Default));
        public static IStream<TOut> SelectMany_Or_Empty<TIn, TOut>(this IStream<Possible<TIn>> stream, Func<TIn, IStream<TOut>> selector) => stream.SelectMany(p => p.Select(selector).s_Value_Or(StreamsCache<TOut>.Empty));
    }
}
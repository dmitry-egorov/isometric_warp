using System;
using Lanski.Structures;

namespace Lanski.Reactive
{
    public static class NullableStreamExtensions
    {
        public static IStream<TOut> Select_Stream_Or_Empty<TIn, TOut>(this Possible<TIn> possible, Func<TIn, IStream<TOut>> selector) => possible.Select(selector).Value_Or(new Stream<TOut>());
        public static IStream<TOut> Select_Stream_Or_Single_Default<TIn, TOut>(this Possible<TIn> possible, Func<TIn, IStream<TOut>> selector) => possible.Select(selector).Value_Or(new Cell<TOut>(default(TOut)));

        public static IStream<T> Value_Or_Empty<T>(this Possible<IStream<T>> possible_stream) => possible_stream.Select(c => c).Value_Or(Stream.Empty<T>());
    }
}
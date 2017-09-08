using System;
using Lanski.Structures;

namespace Lanski.Reactive
{
    public static class VoidStreamExtensions
    {
        public static IStream<TResult> Select<TResult>(this IStream<TheVoid> stream, Func<TResult> selector) => stream.Select(_ => selector()); 
        public static void Next(this IConsumer<TheVoid> consumer) => consumer.Next(TheVoid.Instance);
    }
}
using System;
using Lanski.Structures;

namespace Lanski.Reactive
{
    public static class VoidStreamExtensions
    {
        public static Action Subscribe(this IStream<TheVoid> stream, Action action)
        {
            return stream.Subscribe(_ => action());
        }

        public static void Next(this IConsumer<TheVoid> consumer)
        {
            consumer.Next(TheVoid.Instance);
        }
    }
}
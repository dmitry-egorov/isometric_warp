using System;
using System.Collections.Generic;

namespace Lanski.Reactive
{
    public static class StreamMergeExtensions
    {
        public static IStream<T> Merge<T>(this IStream<IStream<T>> stream) => new MergeStream<T>(stream);

        public class MergeStream<T> : IStream<T>
        {
            private readonly IStream<IStream<T>> _stream;

            public MergeStream(IStream<IStream<T>> stream)
            {
                _stream = stream;
            }

            public Action Subscribe(Action<T> action)
            {
                var subscriptions = new List<Action>();

                var root_subscription = _stream.Subscribe(stream =>
                {
                    subscriptions.Add(stream.Subscribe(action));
                });

                return () =>
                {
                    root_subscription();
                    subscriptions.ForEach(s => s());
                };
            }
        }
    }
}